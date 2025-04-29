namespace SGTD_WebApi.Models.Authenticator;

public class AuthenticatorOtpRequestParams
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string OtpCode { get; set; }
}