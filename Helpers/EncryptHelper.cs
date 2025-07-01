using System.Security.Cryptography;

namespace SGTD_WebApi.Helpers;

/// <summary>
/// Proporciona métodos auxiliares para encriptación y desencriptación de datos, incluyendo contraseñas y archivos.
/// </summary>
public class EncryptHelper
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Inicializa una nueva instancia de la clase EncryptHelper.
    /// </summary>
    /// <param name="configuration">La configuración de la aplicación.</param>
    public EncryptHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Encripta una contraseña utilizando el algoritmo BCrypt con un factor de trabajo de 12.
    /// </summary>
    /// <param name="password">La contraseña en texto plano a encriptar.</param>
    /// <returns>La contraseña encriptada con hash BCrypt.</returns>
    public string PasswordEncrypt(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <summary>
    /// Genera un nombre encriptado único utilizando bytes aleatorios y hash BCrypt.
    /// </summary>
    /// <returns>Un nombre encriptado único con hash BCrypt y factor de trabajo 14.</returns>
    public string EncryptNameGenerator()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        var rawName = Convert.ToBase64String(randomBytes);
        return BCrypt.Net.BCrypt.HashPassword(rawName, workFactor: 14);
    }

    /// <summary>
    /// Encripta el contenido de un archivo utilizando el algoritmo AES con la clave configurada.
    /// </summary>
    /// <param name="input">El stream de entrada que contiene los datos a encriptar.</param>
    /// <returns>Un arreglo de bytes que contiene los datos encriptados.</returns>
    public byte[] FileEncrypt(Stream input)
    {
        var aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(_configuration["Encryption:Key"] ?? string.Empty);
        aesAlg.IV = new byte[16];
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        var msEncrypt = new MemoryStream();
        var cryptoStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        input.CopyTo(cryptoStream);
        cryptoStream.FlushFinalBlock();
        return msEncrypt.ToArray();
    }

    /// <summary>
    /// Desencripta el contenido de un archivo utilizando el algoritmo AES con la clave configurada.
    /// </summary>
    /// <param name="encryptedContent">El arreglo de bytes que contiene los datos encriptados.</param>
    /// <returns>Un arreglo de bytes que contiene los datos desencriptados.</returns>
    public byte[] FileDecrypt(byte[] encryptedContent)
    {
        var aesAlg = Aes.Create();
        aesAlg.Key = Convert.FromBase64String(_configuration["Encryption:Key"] ?? string.Empty);
        aesAlg.IV = new byte[16];
        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        var msDecrypt = new MemoryStream(encryptedContent);
        var cryptoStream = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        var msPlaintext = new MemoryStream();
        cryptoStream.CopyTo(msPlaintext);
        return msPlaintext.ToArray();
    }
}