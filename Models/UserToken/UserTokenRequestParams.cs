namespace SGTD_WebApi.Models.UserToken;

public class UserTokenRequestParams
{
    public Guid UserGuid { get; set; }
    public string Token { get; set; }
}