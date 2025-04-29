using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Helpers;
using SGTD_WebApi.Models.Authenticator;
using static System.Drawing.Imaging.ImageFormat;

namespace SGTD_WebApi.Services.Implementation;

public class AuthenticatorService : IAuthenticatorService
{
    private readonly DatabaseContext _context;
    private readonly AuthenticatorHelper _authenticatorHelper;

    public AuthenticatorService(
        DatabaseContext context, 
        IConfiguration configuration)
    {
        _context = context;
        _authenticatorHelper = new AuthenticatorHelper(configuration);
    }

    public async Task<string> GenerateAuthenticatorKeyAsync(Guid userGuid)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(q => q.UserGuid.Equals(userGuid));

        if (user == null)
        {
            return string.Empty;
        }

        var authenticatorToken = _authenticatorHelper.GenerateAuthenticatorToken(userGuid);
        var secretKey = GenerateSecretKey();

        var authenticator = new Authenticator
        {
            UserGuid = user.UserGuid,
            AuthenticatorToken = authenticatorToken,
            SecretKey = secretKey,
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            IsActive = true
        };

        _context.Authenticators.Add(authenticator);
        await _context.SaveChangesAsync();
        
        return authenticatorToken;
    }

    public async Task<AuthenticatorQRDto> ActivateAuthenticatorToken(string authenticatorToken)
    {
        var userGuid = await _context.Authenticators
            .Where(q => q.AuthenticatorToken == authenticatorToken)
            .Select(q => q.UserGuid)
            .FirstOrDefaultAsync();

        if (userGuid == Guid.Empty)
            throw new InvalidOperationException("Invalid Authenticator Token.");

        var userPerson = await _context.Users
            .Where(q => q.UserGuid.Equals(userGuid))
            .Select(q => new
            {
                q.Email,
                FullName = $"{q.Person.FirstName} {q.Person.LastName}"
            })
            .FirstOrDefaultAsync();

        if (userPerson == null)
            throw new InvalidOperationException("Invalid User.");

        var secretKey = await _context.Authenticators
            .Where(q =>
                q.UserGuid == userGuid
                && q.IsActive
                && q.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(q => q.Id)
            .Select(q => q.SecretKey)
            .FirstOrDefaultAsync();

        if (secretKey == null)
            throw new InvalidOperationException("Invalid Secret Key.");

        var appName = "SGTD-App";
        var qrCodeUri = GenerateQrCodeUri(userPerson.Email, secretKey, appName);

        var base64Qr = GenerateBase64Qr(qrCodeUri);

        return new AuthenticatorQRDto
        {
            FullName = userPerson.FullName,
            QrCodeImage = $"data:image/png;base64,{base64Qr}"
        };
    }

    public async Task<bool> VerifyAuthenticatorOtpAsync(AuthenticatorOtpRequestParams requestParams)
    {
        var userGuid= await _context.Users
            .Where(q => q.Email == requestParams.Email)
            .Select(q => q.UserGuid)
            .FirstOrDefaultAsync();

        if (userGuid == Guid.Empty)
            throw new InvalidOperationException("Invalid User.");

        var secretKey = await _context.Authenticators
            .Where(q =>
                q.UserGuid == userGuid
                && q.IsActive)
                //&& q.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(q => q.Id)
            .Select(q => q.SecretKey)
            .FirstOrDefaultAsync();

        if (secretKey == null)
            throw new InvalidOperationException("Invalid Secret Key.");

        var bytes = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(bytes);

        var isValid = totp.VerifyTotp(
            requestParams.OtpCode,
            out _,
            new VerificationWindow(previous: 1, future: 1)
        );

        return isValid;
    }

    private string GenerateBase64Qr(string qrCodeUri)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCode(qrCodeData);
        using var qrBitmap = qrCode.GetGraphic(20);
        using var stream = new MemoryStream();
        qrBitmap.Save(stream, Png);
        return Convert.ToBase64String(stream.ToArray());
    }

    public string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string email, string secretKey, string appName)
    {
        return $"otpauth://totp/{appName}:{email}?secret={secretKey}&issuer={appName}&digits=6";
    }
}