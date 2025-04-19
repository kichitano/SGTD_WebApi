using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.Models.Auth;
using SGTD_WebApi.Models.UserToken;

namespace SGTD_WebApi.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _context;
    private readonly IUserTokenService _userTokenService;

    public AuthService(DatabaseContext context, IUserTokenService userTokenService)
    {
        _context = context;
        _userTokenService = userTokenService;
    }

    public async Task<AuthDto> LoginAsync(AuthRequestParams requestParams)
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

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return await _userTokenService.ValidateTokenAsync(token);
    }

    public CookieOptions SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        return cookieOptions;
    }
}