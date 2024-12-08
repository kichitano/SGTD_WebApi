using System.Security.Cryptography;
using System.Text;

namespace SGTD_WebApi.Helpers;

public class EncryptHelper
{
    private readonly IConfiguration _configuration;

    public EncryptHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string PasswordEncrypt(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

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