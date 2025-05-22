namespace SGTD_WebApi.Services;

public interface IUserDigitalSignatureService
{
    Task UploadDigitalSignatureAsync(IFormFile userDigitalSignature, Guid userGuid);
    Task<bool> VerifyUserDigitalSignatureAsync(Guid userGuid);
}