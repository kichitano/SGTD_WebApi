using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Services;

public interface IAuthenticatorEmailService
{
    Task SendAuthenticatorEmailAsync(AuthenticatorEmailRequestParams request);
}