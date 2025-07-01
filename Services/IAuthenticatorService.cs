using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de autenticación de dos factores.
/// </summary>
public interface IAuthenticatorService
{
    /// <summary>
    /// Genera una clave de autenticación para un usuario.
    /// </summary>
    /// <param name="userGuid">GUID del usuario.</param>
    /// <returns>Clave de autenticación generada.</returns>
    Task<string> GenerateAuthenticatorKeyAsync(Guid userGuid);
    /// <summary>
    /// Activa el token de autenticación y genera el código QR.
    /// </summary>
    /// <param name="authenticatorToken">Token de autenticación.</param>
    /// <returns>Datos del código QR para la autenticación.</returns>
    Task<AuthenticatorQRDto> ActivateAuthenticatorToken(string authenticatorToken);
    /// <summary>
    /// Verifica un código OTP de autenticación de dos factores.
    /// </summary>
    /// <param name="requestParams">Parámetros del código OTP.</param>
    /// <returns>True si el código es válido.</returns>
    Task<bool> VerifyAuthenticatorOtpAsync(AuthenticatorOtpRequestParams requestParams);
}