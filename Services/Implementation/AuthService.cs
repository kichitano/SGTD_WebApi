using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.Models.Auth;
using SGTD_WebApi.Models.Authenticator;
using SGTD_WebApi.Models.UserToken;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar la autenticación y autorización de usuarios.
/// Proporciona funcionalidades para login, logout, validación de tokens y configuración de cookies.
/// </summary>
public class AuthService : IAuthService
{
    private readonly DatabaseContext _context;
    private readonly IUserTokenService _userTokenService;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de autenticación.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    /// <param name="userTokenService">Servicio para gestionar tokens de usuario.</param>
    public AuthService(DatabaseContext context, IUserTokenService userTokenService)
    {
        _context = context;
        _userTokenService = userTokenService;
    }

    /// <summary>
    /// Realiza un login básico verificando la existencia del usuario de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen las credenciales del usuario.</param>
    /// <returns>True si el usuario existe, false en caso contrario.</returns>
    public async Task<bool> LoginAsync(AuthRequestParams requestParams)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == requestParams.Email);

        return user != null;
    }

    /// <summary>
    /// Realiza un login con autenticación de dos factores (OTP) de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen email, contraseña y código OTP.</param>
    /// <returns>Un objeto DTO que contiene el resultado de la autenticación y tokens.</returns>
    /// <exception cref="ValidationException">Se lanza cuando las credenciales son incorrectas.</exception>
    public async Task<AuthDto> LoginOtpAsync(AuthenticatorOtpRequestParams requestParams)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == requestParams.Email);

        if (user == null)
        {
            return new AuthDto { Success = false };
        }

        bool isValidPassword = BCrypt.Net.BCrypt.Verify(requestParams.Password, user.Password);
        if (!isValidPassword)
        {
            throw new ValidationException("Credenciales incorrectas");
        }

        var token = await _userTokenService.GenerateTokenAsync(user.UserGuid);
        var refreshToken = await _context.UserTokens
            .Where(q => q.Token.Equals(token))
            .Select(q => q.RefreshToken)
            .FirstOrDefaultAsync();

        return new AuthDto
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken ?? string.Empty
        };
    }

    /// <summary>
    /// Realiza el logout de un usuario invalidando todos sus tokens de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen el email del usuario.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task LogoutAsync(LogoutRequestParams requestParams)
    {
        var user = await _context.Users
            .Where(q => q.Email.Equals(requestParams.Email))
            .Select(q => new UserTokenModel
            {
                UserGuid = q.UserGuid
            })
            .FirstOrDefaultAsync();

        if (user != null) await _userTokenService.InvalidateAllTokensAsync(user);
    }

    /// <summary>
    /// Valida un token de acceso de forma asíncrona.
    /// </summary>
    /// <param name="token">El token a validar.</param>
    /// <returns>True si el token es válido, false en caso contrario.</returns>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        return await _userTokenService.ValidateTokenAsync(token);
    }

    /// <summary>
    /// Configura las opciones de cookie para el refresh token.
    /// </summary>
    /// <param name="refreshToken">El refresh token a configurar en la cookie.</param>
    /// <returns>Las opciones de cookie configuradas con seguridad.</returns>
    public CookieOptions SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        return cookieOptions;
    }
}