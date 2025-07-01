using SGTD_WebApi.Models.UserToken;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de tokens de usuarios.
/// Proporciona funcionalidades para generar, validar y gestionar tokens de autenticación.
/// </summary>
public interface IUserTokenService
{
    /// <summary>
    /// Genera un nuevo token de autenticación para un usuario de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>Token de autenticación generado</returns>
    Task<string> GenerateTokenAsync(Guid userGuid);
    /// <summary>
    /// Invalida todos los tokens de un usuario de forma asíncrona.
    /// </summary>
    /// <param name="userTokenModel">Modelo con la información del token del usuario</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task InvalidateAllTokensAsync(UserTokenModel userTokenModel);
    /// <summary>
    /// Valida si un token es válido de forma asíncrona.
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>True si el token es válido, False en caso contrario</returns>
    Task<bool> ValidateTokenAsync(string token);
    /// <summary>
    /// Obtiene el identificador del usuario a partir de un token de actualización de forma asíncrona.
    /// </summary>
    /// <param name="refreshToken">Token de actualización</param>
    /// <returns>Identificador del usuario o null si el token no es válido</returns>
    Task<Guid?> GetUserGuidFromRefreshTokenAsync(string refreshToken);
    /// <summary>
    /// Obtiene el token de actualización del usuario a partir de un token generado de forma asíncrona.
    /// </summary>
    /// <param name="token">Token generado</param>
    /// <returns>Token de actualización del usuario</returns>
    Task<string> GetUserRefreshTokenFromGeneratedTokenAsync(string token);
    /// <summary>
    /// Revoca un token de actualización de forma asíncrona.
    /// </summary>
    /// <param name="refreshToken">Token de actualización a revocar</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task RevokeRefreshTokenAsync(string refreshToken);
}