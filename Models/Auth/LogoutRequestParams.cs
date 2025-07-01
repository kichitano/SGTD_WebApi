namespace SGTD_WebApi.Models.Auth;

/// <summary>
/// Parámetros de solicitud para cerrar sesión de usuario
/// </summary>
public class LogoutRequestParams
{
    public string Email { get; set; }
}