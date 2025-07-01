namespace SGTD_WebApi.Models.Authenticator;

/// <summary>
/// Parámetros de solicitud para autenticación por correo electrónico
/// </summary>
public class AuthenticatorEmailRequestParams
{
    public string Email { get; set; }
}