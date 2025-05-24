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
}