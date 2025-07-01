namespace SGTD_WebApi.Models.Authenticator;

/// <summary>
/// Parámetros de solicitud para autenticación con código OTP
/// </summary>
public class AuthenticatorOtpRequestParams
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string OtpCode { get; set; }
}