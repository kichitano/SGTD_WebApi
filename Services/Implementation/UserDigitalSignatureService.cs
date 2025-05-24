using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Helpers;

namespace SGTD_WebApi.Services.Implementation;

public class UserDigitalSignatureService : IUserDigitalSignatureService
{
    private readonly DatabaseContext _context;
    private readonly EncryptHelper _encryptHelper;
    private readonly string _basePath;

    public UserDigitalSignatureService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _encryptHelper = new EncryptHelper(configuration);
        _basePath = configuration["FilesPath:SignaturesPath"] ?? string.Empty;
    }

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

    public async Task<bool> VerifyUserDigitalSignatureAsync(Guid userGuid)
    {
        var userDigitalSignatureExist = await _context.UserDigitalSignatures
            .Where(q => q.UserGuid == userGuid)
            .FirstOrDefaultAsync();

        return userDigitalSignatureExist != null;
    }

}