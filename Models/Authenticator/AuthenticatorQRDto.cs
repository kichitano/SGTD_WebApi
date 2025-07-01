namespace SGTD_WebApi.Models.Authenticator;

/// <summary>
/// Representa los datos de transferencia para generar código QR de autenticación
/// </summary>
public class AuthenticatorQRDto
{
    public string FullName { get; set; }
    public string QrCodeImage { get; set; }
}