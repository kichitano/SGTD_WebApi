using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Helpers;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar firmas digitales de usuarios.
/// Proporciona funcionalidades para subir y verificar firmas digitales con encriptación de archivos.
/// </summary>
public class UserDigitalSignatureService : IUserDigitalSignatureService
{
    private readonly DatabaseContext _context;
    private readonly EncryptHelper _encryptHelper;
    private readonly string _basePath;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de firmas digitales de usuarios.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    /// <param name="configuration">Configuración de la aplicación para acceder a rutas y configuraciones de encriptación.</param>
    public UserDigitalSignatureService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _encryptHelper = new EncryptHelper(configuration);
        _basePath = configuration["FilesPath:SignaturesPath"] ?? string.Empty;
    }

    /// <summary>
    /// Sube una firma digital para un usuario de forma asíncrona.
    /// El archivo se encripta antes de almacenarse en el sistema de archivos.
    /// </summary>
    /// <param name="userDigitalSignature">El archivo de imagen de la firma digital a subir.</param>
    /// <param name="userGuid">El identificador único del usuario propietario de la firma.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ValidationException">Se lanza cuando el usuario no se encuentra.</exception>
    /// <exception cref="FileNotFoundException">Se lanza cuando no se puede guardar la firma digital correctamente.</exception>
    public async Task UploadDigitalSignatureAsync(IFormFile userDigitalSignature, Guid userGuid)
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

        var digitalSignatureName = _encryptHelper.EncryptNameGenerator();
        var filePath = Path.Combine(folderPath, digitalSignatureName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            var encryptedContent = _encryptHelper.FileEncrypt(userDigitalSignature.OpenReadStream());
            await stream.WriteAsync(encryptedContent, 0, encryptedContent.Length);
        }

        if (Path.Exists(filePath))
        {
            var digitalSignature = new UserDigitalSignature
            {
                Name = digitalSignatureName,
                UserGuid = userGuid,
                CreatedAt = DateTime.UtcNow
            };
            await _context.UserDigitalSignatures.AddAsync(digitalSignature);
        }
        else
        {
            throw new FileNotFoundException("No se pudo guardar la firma digital correctamente.");
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Verifica si un usuario tiene una firma digital registrada de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">El identificador único del usuario a verificar.</param>
    /// <returns>True si el usuario tiene una firma digital registrada, false en caso contrario.</returns>
    public async Task<bool> VerifyUserDigitalSignatureAsync(Guid userGuid)
    {
        var userDigitalSignatureExist = await _context.UserDigitalSignatures
            .Where(q => q.UserGuid == userGuid)
            .FirstOrDefaultAsync();

        return userDigitalSignatureExist != null;
    }

}