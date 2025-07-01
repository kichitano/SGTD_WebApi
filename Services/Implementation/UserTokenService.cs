using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Helpers;
using SGTD_WebApi.Models.UserToken;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar tokens de autenticación de usuarios.
/// Proporciona funcionalidades para generar, validar, renovar y revocar tokens JWT y refresh tokens.
/// </summary>
public class UserTokenService : IUserTokenService
{
    private readonly DatabaseContext _context;
    private readonly JwtHelper _jwtHelper;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de tokens de usuarios.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    /// <param name="configuration">Configuración de la aplicación para configuraciones JWT.</param>
    public UserTokenService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _jwtHelper = new JwtHelper(configuration);
    }

    /// <summary>
    /// Genera un nuevo token JWT para un usuario de forma asíncrona.
    /// Invalida todos los tokens previos del usuario excepto el recién generado.
    /// </summary>
    /// <param name="userGuid">El identificador único del usuario.</param>
    /// <returns>El token JWT generado, o cadena vacía si el usuario no existe.</returns>
    public async Task<string> GenerateTokenAsync(Guid userGuid)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(q => q.UserGuid.Equals(userGuid));

        if (user == null)
        {
            return string.Empty;
        }

        var token = _jwtHelper.GenerateJwtToken(user);
        var refreshToken = Guid.NewGuid().ToString();

        var userToken = new UserToken
        {
            UserGuid = user.UserGuid,
            Token = token,
            RefreshToken = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsActive = true
        };

        _context.UserTokens.Add(userToken);
        await _context.SaveChangesAsync();

        await InvalidateAllTokensAsync(new UserTokenModel
        { UserGuid = userToken.UserGuid, Token = userToken.Token });

        return token;
    }

    /// <summary>
    /// Obtiene el identificador de usuario a partir de un refresh token de forma asíncrona.
    /// </summary>
    /// <param name="refreshToken">El refresh token a validar.</param>
    /// <returns>El GUID del usuario si el refresh token es válido, null en caso contrario.</returns>
    public async Task<Guid?> GetUserGuidFromRefreshTokenAsync(string refreshToken)
    {
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken && t.IsActive);
        return userToken?.UserGuid;
    }

    /// <summary>
    /// Obtiene y renueva el refresh token asociado a un token JWT de forma asíncrona.
    /// </summary>
    /// <param name="token">El token JWT para el cual se solicita el refresh token.</param>
    /// <returns>Un nuevo refresh token si el token es válido, cadena vacía en caso contrario.</returns>
    public async Task<string> GetUserRefreshTokenFromGeneratedTokenAsync(string token)
    {
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.IsActive && t.ExpiresAt > DateTime.UtcNow);

        if (userToken == null)
            return string.Empty;

        var newRefreshToken = Guid.NewGuid().ToString();
        userToken.RefreshToken = newRefreshToken;
        userToken.ExpiresAt = DateTime.UtcNow.AddDays(15);
        await _context.SaveChangesAsync();

        return newRefreshToken;
    }

    /// <summary>
    /// Invalida todos los tokens de un usuario de forma asíncrona.
    /// Opcionalmente excluye un token específico de la invalidación.
    /// </summary>
    /// <param name="userTokenModel">Modelo que contiene el GUID del usuario y opcionalmente un token a excluir.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task InvalidateAllTokensAsync(UserTokenModel userTokenModel)
    {
        var userTokens = await _context.UserTokens
            .Where(q => q.UserGuid.Equals(userTokenModel.UserGuid))
            .ToListAsync();

        if (!string.IsNullOrEmpty(userTokenModel.Token))
        {
            userTokens = userTokens.Where(q => q.Token != userTokenModel.Token).ToList();
        }

        if (userTokens.Count > 0)
        {
            foreach (var userToken in userTokens)
            {
                userToken.IsActive = false;
            }
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Valida un token JWT verificando su existencia, estado activo y fecha de expiración de forma asíncrona.
    /// </summary>
    /// <param name="token">El token JWT a validar.</param>
    /// <returns>True si el token es válido y activo, false en caso contrario.</returns>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        var response = false;
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t =>
                t.Token == token);

        if (userToken == null)
        {
            return false;
        }

        if (userToken.IsActive && userToken.ExpiresAt > DateTime.UtcNow)
        {
            response = true;
        }
        else
        {
            await InvalidateAllTokensAsync(new UserTokenModel
                { UserGuid = userToken.UserGuid, Token = userToken.Token });
        }
        return response;
    }

    /// <summary>
    /// Revoca un refresh token específico marcándolo como inactivo de forma asíncrona.
    /// </summary>
    /// <param name="refreshToken">El refresh token a revocar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);

        if (userToken != null)
        {
            userToken.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}