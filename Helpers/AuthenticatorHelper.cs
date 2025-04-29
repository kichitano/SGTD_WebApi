using System.Security.Cryptography;
using System.Text;

namespace SGTD_WebApi.Helpers;

public class AuthenticatorHelper
{
    private readonly IConfiguration _configuration;

    public AuthenticatorHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAuthenticatorToken(Guid userGuid)
    {
        var key = _configuration["Authenticator:Key"] ?? string.Empty;
        var uniqueString = $"{key}_{userGuid}_{DateTime.UtcNow.Ticks}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(uniqueString));
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append($"{b:x2}");
        }
        return sb.ToString();
    }
}