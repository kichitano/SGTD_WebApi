namespace SGTD_WebApi.Models.Auth;

/// <summary>
/// Representa la respuesta de autenticación con tokens de acceso
/// </summary>
public class AuthDto
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}