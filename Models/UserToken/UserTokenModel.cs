namespace SGTD_WebApi.Models.UserToken;

/// <summary>
/// Modelo que representa un token de usuario en el sistema.
/// </summary>
public class UserTokenModel
{
    public Guid UserGuid { get; set; }
    public string? Token { get; set; }
}