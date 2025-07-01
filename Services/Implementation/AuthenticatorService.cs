using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Helpers;
using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar la autenticación de dos factores (2FA).
/// Proporciona funcionalidades para generar claves secretas, tokens de activación, códigos QR y verificación OTP.
/// </summary>
public class AuthenticatorService : IAuthenticatorService
{
    private readonly DatabaseContext _context;
    private readonly AuthenticatorHelper _authenticatorHelper;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de autenticación de dos factores.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    /// <param name="configuration">Configuración de la aplicación para el helper de autenticación.</param>
    public AuthenticatorService(
        DatabaseContext context, 
        IConfiguration configuration)
    {
        _context = context;
        _authenticatorHelper = new AuthenticatorHelper(configuration);
    }

    /// <summary>
    /// Genera un token de activación para autenticador de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">El identificador único del usuario.</param>
    /// <returns>El token de activación generado o cadena vacía si el usuario no existe.</returns>
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

    /// <summary>
    /// Activa un token de autenticador y genera un código QR para configuración de forma asíncrona.
    /// </summary>
    /// <param name="authenticatorToken">El token de activación del autenticador.</param>
    /// <returns>Un objeto DTO que contiene el nombre completo del usuario y la imagen QR en base64.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el token, usuario o clave secreta son inválidos.</exception>
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

    /// <summary>
    /// Verifica un código OTP del autenticador de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen el email del usuario y el código OTP.</param>
    /// <returns>True si el código OTP es válido, false en caso contrario.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el usuario o la clave secreta son inválidos.</exception>
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

    /// <summary>
    /// Genera una imagen QR en formato base64 a partir de una URI.
    /// </summary>
    /// <param name="qrCodeUri">La URI para generar el código QR.</param>
    /// <returns>La imagen QR codificada en base64.</returns>
    private string GenerateBase64Qr(string qrCodeUri)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeBytes);
    }

    /// <summary>
    /// Genera una clave secreta aleatoria para autenticación de dos factores.
    /// </summary>
    /// <returns>La clave secreta codificada en Base32.</returns>
    public string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    /// <summary>
    /// Genera una URI para código QR de autenticación TOTP.
    /// </summary>
    /// <param name="email">El email del usuario.</param>
    /// <param name="secretKey">La clave secreta para autenticación.</param>
    /// <param name="appName">El nombre de la aplicación.</param>
    /// <returns>La URI formateada para generar el código QR.</returns>
    public string GenerateQrCodeUri(string email, string secretKey, string appName)
    {
        return $"otpauth://totp/{appName}:{email}?secret={secretKey}&issuer={appName}&digits=6";
    }
}