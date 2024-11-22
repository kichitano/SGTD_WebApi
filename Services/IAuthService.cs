using SGTD_WebApi.Models.Auth;

namespace SGTD_WebApi.Services;

public interface IAuthService
{
    Task<AuthDto> LoginAsync(AuthRequestParams requestParams);
    Task LogoutAsync(LogoutRequestParams requestParams);
    Task<bool> ValidateTokenAsync(string token);
    CookieOptions SetRefreshTokenCookie(string refreshToken);
}