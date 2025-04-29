using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Services;

public interface IAuthenticatorService
{
    Task<string> GenerateAuthenticatorKeyAsync(Guid userGuid);
    Task<AuthenticatorQRDto> ActivateAuthenticatorToken(string authenticatorToken);
    Task<bool> VerifyAuthenticatorOtpAsync(AuthenticatorOtpRequestParams requestParams);
}