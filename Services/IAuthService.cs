using SGTD_WebApi.Models.Auth;
using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(AuthRequestParams requestParams);
    Task<AuthDto> LoginOtpAsync(AuthenticatorOtpRequestParams requestParams);
    Task LogoutAsync(LogoutRequestParams requestParams);
    CookieOptions SetRefreshTokenCookie(string refreshToken);
}