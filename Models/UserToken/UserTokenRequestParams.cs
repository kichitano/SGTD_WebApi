namespace SGTD_WebApi.Models.UserToken;

/// <summary>
/// Parámetros de solicitud para operaciones de tokens de usuario.
/// </summary>
public class UserTokenRequestParams
{
    public Guid UserGuid { get; set; }
    public string Token { get; set; }
}