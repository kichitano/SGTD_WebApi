using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.Models.UserFile;
using SGTD_WebApi.DbModels.Entities;
using System.IO.Compression;
using SGTD_WebApi.Helpers;

namespace SGTD_WebApi.Services.Implementation;

public class UserFileService : IUserFileService
{
    private readonly DatabaseContext _context;
    private readonly EncryptHelper _encryptHelper;
    private readonly string _basePath;

    public UserFileService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _encryptHelper = new EncryptHelper(configuration);
        _basePath = configuration["FilesPath:BasePath"] ?? string.Empty;
    }

    public async Task<List<UserFileDto>> GetByUserGuIdAsync(Guid userGuid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserGuid == userGuid);

        if (user == null)
        {
            throw new ValidationException("Usuario no encontrado");
        }

        return await _context.UserFiles
            .Where(q => q.User.UserGuid.Equals(userGuid))
            .Select(q => new UserFileDto
            {
                Id = q.Id,
                FileName = q.FileName,
                FileSize = q.FileSize,
                ContentType = q.ContentType,
                CreatedAt = q.CreatedAt
            })
            .ToListAsync();
    }

    public async Task UploadFilesAsync(List<IFormFile> userFiles, Guid userGuid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserGuid == userGuid);

        if (user == null)
        {
            throw new ValidationException("Usuario no encontrado");
        }

        var folderPath = Path.Combine(_basePath, user.FolderPath);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        foreach (var userFile in userFiles)
        {
            var fileName = userFile.FileName;
            var filePath = Path.Combine(folderPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var encryptedContent = _encryptHelper.FileEncrypt(userFile.OpenReadStream());
                await stream.WriteAsync(encryptedContent, 0, encryptedContent.Length);
            }

            if (Path.Exists(filePath))
            {
                var file = new UserFile
                {
                    UserId = user.Id,
                    FileName = userFile.FileName,
                    FileSize = userFile.Length,
                    ContentType = Path.GetExtension(fileName).TrimStart('.'),
                    CreatedAt = DateTime.UtcNow
                };
                await _context.UserFiles.AddAsync(file);
            }
            else
            {
                throw new FileNotFoundException("No se pudo guardar el archivo correctamente.");
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<UserFileByteDto> DownloadFileAsync(int id)
    {
        var userFile = await _context.UserFiles
            .Where(q => q.Id == id)
            .Select(q => new
            {
                q.FileName,
                q.User.FolderPath
            })
            .FirstOrDefaultAsync();

        var filePath = Path.Combine(_basePath, userFile?.FolderPath ?? string.Empty);
        var file = Directory.GetFiles(filePath).FirstOrDefault(f => userFile != null && Path.GetFileName(f).Equals(userFile.FileName));

        if (file == null)
            throw new FileNotFoundException("File not found");

        var encryptedContent = await File.ReadAllBytesAsync(file);
        var decryptedContent = _encryptHelper.FileDecrypt(encryptedContent);

        return new UserFileByteDto
        {
            File = decryptedContent,
            FileName = userFile?.FileName ?? string.Empty
        };
    }

    public async Task<byte[]> DownloadMultipleFilesAsync(List<int> ids)
    {
        using var zipMemoryStream = new MemoryStream();
        using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var id in ids)
            {
                var file = await DownloadFileAsync(id);
                var zipEntry = zipArchive.CreateEntry(file.FileName);
                await using var entryStream = zipEntry.Open();
                await entryStream.WriteAsync(file.File, 0, file.File.Length);
            }
        }
        return zipMemoryStream.ToArray();
    }

    public async Task<string> DeleteFileAsync(int id, Guid userGuid)
    {
        // Verificar que el usuario existe primero
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserGuid == userGuid);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado");
        }

        var userFile = await _context.UserFiles
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (userFile == null)
        {
            return $"Archivo con ID {id} no existe en la base de datos (ya fue eliminado previamente)";
        }

        // Verificar que el usuario es el propietario del archivo
        if (userFile.User.UserGuid != userGuid)
        {
            throw new UnauthorizedAccessException("No tiene permisos para eliminar este archivo");
        }

        var filePath = Path.Combine(_basePath, userFile.User.FolderPath, userFile.FileName);
        var fileExists = File.Exists(filePath);

        // Eliminar registro de la base de datos
        _context.UserFiles.Remove(userFile);
        await _context.SaveChangesAsync();

        // Intentar eliminar archivo físico si existe
        if (fileExists)
        {
            try
            {
                File.Delete(filePath);
                return "Archivo eliminado correctamente";
            }
            catch (Exception ex)
            {
                return $"Registro eliminado de la base de datos, pero no se pudo eliminar el archivo físico: {ex.Message}";
            }
        }
        else
        {
            return "Registro eliminado de la base de datos. Advertencia: El archivo físico no existía";
        }
    }

    public async Task<string> DeleteMultipleFilesAsync(List<int> ids, Guid userGuid)
    {
        // Verificar que el usuario existe primero
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserGuid == userGuid);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado");
        }

        var userFiles = await _context.UserFiles
            .Include(f => f.User)
            .Where(f => ids.Contains(f.Id))
            .ToListAsync();

        if (!userFiles.Any())
        {
            return $"Ninguno de los {ids.Count} archivos seleccionados existe en la base de datos (ya fueron eliminados previamente)";
        }

        // Verificar que todos los archivos pertenecen al usuario
        var unauthorizedFiles = userFiles.Where(f => f.User.UserGuid != userGuid).ToList();
        if (unauthorizedFiles.Any())
        {
            throw new UnauthorizedAccessException("No tiene permisos para eliminar algunos archivos seleccionados");
        }

        var results = new List<string>();
        var deletedCount = 0;
        var warningCount = 0;
        var errorCount = 0;

        foreach (var userFile in userFiles)
        {
            var filePath = Path.Combine(_basePath, userFile.User.FolderPath, userFile.FileName);
            var fileExists = File.Exists(filePath);

            // Eliminar registro de la base de datos
            _context.UserFiles.Remove(userFile);

            // Intentar eliminar archivo físico si existe
            if (fileExists)
            {
                try
                {
                    File.Delete(filePath);
                    deletedCount++;
                }
                catch
                {
                    errorCount++;
                }
            }
            else
            {
                warningCount++;
            }
        }

        await _context.SaveChangesAsync();

        var result = $"Eliminación completada: {deletedCount} archivos eliminados correctamente";
        if (warningCount > 0)
        {
            result += $", {warningCount} registros eliminados (archivos físicos no existían)";
        }
        if (errorCount > 0)
        {
            result += $", {errorCount} archivos con errores al eliminar físicamente";
        }

        return result;
    }

    public async Task<FileShareInfoDto> GetFileShareInfoAsync(int fileId, Guid userGuid)
    {
        var userFile = await _context.UserFiles
            .Include(f => f.User)
            .ThenInclude(u => u.Person)
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (userFile == null)
        {
            throw new ValidationException("Archivo no encontrado");
        }

        // Verificar que el usuario es el propietario o tiene acceso compartido
        if (userFile.User.UserGuid != userGuid)
        {
            var hasSharedAccess = await _context.UserFileShares
                .Include(s => s.SharedWithUser)
                .AnyAsync(s => s.UserFileId == fileId && s.SharedWithUser.UserGuid == userGuid);

            if (!hasSharedAccess)
            {
                throw new UnauthorizedAccessException("No tiene permisos para ver la información de este archivo");
            }
        }

        var sharedUsers = await _context.UserFileShares
            .Include(s => s.SharedWithUser)
            .ThenInclude(u => u.Person)
            .Include(s => s.SharedByUser)
            .ThenInclude(u => u.Person)
            .Where(s => s.UserFileId == fileId)
            .Select(s => new FileShareUserDto
            {
                UserId = s.SharedWithUserId,
                Name = $"{s.SharedWithUser.Person.FirstName} {s.SharedWithUser.Person.LastName}",
                SharedAt = s.SharedAt,
                SharedByName = $"{s.SharedByUser.Person.FirstName} {s.SharedByUser.Person.LastName}"
            })
            .ToListAsync();

        return new FileShareInfoDto
        {
            FileId = fileId,
            OwnerName = $"{userFile.User.Person.FirstName} {userFile.User.Person.LastName}",
            SharedUsers = sharedUsers
        };
    }

    public async Task<string> ShareFileAsync(int fileId, List<int> userIds, Guid sharedByUserGuid)
    {
        var userFile = await _context.UserFiles
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (userFile == null)
        {
            throw new ValidationException("Archivo no encontrado");
        }

        // Verificar que el usuario es el propietario del archivo
        if (userFile.User.UserGuid != sharedByUserGuid)
        {
            throw new UnauthorizedAccessException("Solo el propietario puede compartir este archivo");
        }

        var sharedByUser = await _context.Users
            .FirstOrDefaultAsync(u => u.UserGuid == sharedByUserGuid);

        if (sharedByUser == null)
        {
            throw new ValidationException("Usuario que comparte no encontrado");
        }

        // Verificar que los usuarios a compartir existen
        var usersToShare = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        if (usersToShare.Count != userIds.Count)
        {
            throw new ValidationException("Algunos usuarios seleccionados no existen");
        }

        // Verificar que no se intente compartir consigo mismo
        if (usersToShare.Any(u => u.Id == sharedByUser.Id))
        {
            throw new ValidationException("No puede compartir un archivo consigo mismo");
        }

        // Obtener compartidos existentes
        var existingShares = await _context.UserFileShares
            .Where(s => s.UserFileId == fileId && userIds.Contains(s.SharedWithUserId))
            .Select(s => s.SharedWithUserId)
            .ToListAsync();

        // Crear nuevos compartidos solo para usuarios que no tienen acceso
        var newShares = userIds.Except(existingShares).ToList();

        foreach (var userId in newShares)
        {
            await _context.UserFileShares.AddAsync(new UserFileShare
            {
                UserFileId = fileId,
                SharedWithUserId = userId,
                SharedByUserId = sharedByUser.Id,
                SharedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        var message = $"Archivo compartido con {newShares.Count} usuario(s)";
        if (existingShares.Any())
        {
            message += $". {existingShares.Count} usuario(s) ya tenían acceso";
        }

        return message;
    }

    public async Task<string> UnshareFileAsync(int fileId, int userId, Guid userGuid)
    {
        var userFile = await _context.UserFiles
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (userFile == null)
        {
            throw new ValidationException("Archivo no encontrado");
        }

        // Verificar que el usuario es el propietario del archivo
        if (userFile.User.UserGuid != userGuid)
        {
            throw new UnauthorizedAccessException("Solo el propietario puede dejar de compartir este archivo");
        }

        var shareRecord = await _context.UserFileShares
            .FirstOrDefaultAsync(s => s.UserFileId == fileId && s.SharedWithUserId == userId);

        if (shareRecord == null)
        {
            throw new ValidationException("El archivo no está compartido con este usuario");
        }

        _context.UserFileShares.Remove(shareRecord);
        await _context.SaveChangesAsync();

        return "Acceso compartido eliminado correctamente";
    }

    public async Task<List<UserFileShareDto>> GetSharedFilesAsync(Guid userGuid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserGuid == userGuid);

        if (user == null)
        {
            throw new ValidationException("Usuario no encontrado");
        }

        var sharedFiles = await _context.UserFileShares
            .Include(s => s.UserFile)
            .ThenInclude(f => f.User)
            .ThenInclude(u => u.Person)
            .Include(s => s.SharedByUser)
            .ThenInclude(u => u.Person)
            .Where(s => s.SharedWithUserId == user.Id)
            .Select(s => new UserFileShareDto
            {
                Id = s.UserFile.Id,
                FileName = s.UserFile.FileName,
                FileSize = s.UserFile.FileSize,
                ContentType = s.UserFile.ContentType,
                CreatedAt = s.UserFile.CreatedAt,
                OwnerName = $"{s.UserFile.User.Person.FirstName} {s.UserFile.User.Person.LastName}",
                SharedByName = $"{s.SharedByUser.Person.FirstName} {s.SharedByUser.Person.LastName}",
                SharedAt = s.SharedAt,
                IsOwner = false
            })
            .ToListAsync();

        return sharedFiles;
    }
}