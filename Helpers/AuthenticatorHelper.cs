using System.Security.Cryptography;
using System.Text;

namespace SGTD_WebApi.Helpers;

/// <summary>
/// Proporciona métodos auxiliares para la generación de tokens de autenticación de dos factores.
/// </summary>
public class AuthenticatorHelper
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Inicializa una nueva instancia de la clase AuthenticatorHelper.
    /// </summary>
    /// <param name="configuration">La configuración de la aplicación.</param>
    public AuthenticatorHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Genera un token único de autenticación basado en el GUID del usuario y un timestamp.
    /// </summary>
    /// <param name="userGuid">El identificador único del usuario.</param>
    /// <returns>Un token de autenticación cifrado con SHA256 en formato hexadecimal.</returns>
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