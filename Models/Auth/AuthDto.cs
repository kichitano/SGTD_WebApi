namespace SGTD_WebApi.Models.Auth;

public class AuthDto
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}