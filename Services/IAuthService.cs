using SGTD_WebApi.Models.Auth;
using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de autenticación de usuarios.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Inicia el proceso de autenticación de un usuario.
    /// </summary>
    /// <param name="requestParams">Parámetros de autenticación.</param>
    /// <returns>True si el proceso se inicia correctamente.</returns>
    Task<bool> LoginAsync(AuthRequestParams requestParams);
    /// <summary>
    /// Completa la autenticación usando código OTP.
    /// </summary>
    /// <param name="requestParams">Parámetros del código OTP.</param>
    /// <returns>Datos de autenticación del usuario.</returns>
    Task<AuthDto> LoginOtpAsync(AuthenticatorOtpRequestParams requestParams);
    /// <summary>
    /// Cierra la sesión de un usuario.
    /// </summary>
    /// <param name="requestParams">Parámetros de cierre de sesión.</param>
    Task LogoutAsync(LogoutRequestParams requestParams);
    /// <summary>
    /// Configura las opciones de cookie para el token de actualización.
    /// </summary>
    /// <param name="refreshToken">Token de actualización.</param>
    /// <returns>Opciones de cookie configuradas.</returns>
    CookieOptions SetRefreshTokenCookie(string refreshToken);
}