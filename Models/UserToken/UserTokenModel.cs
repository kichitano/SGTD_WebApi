namespace SGTD_WebApi.Models.UserToken;

public class UserTokenModel
{
    public Guid UserGuid { get; set; }
    public string? Token { get; set; }
}