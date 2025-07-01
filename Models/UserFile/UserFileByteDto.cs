namespace SGTD_WebApi.Models.UserFile;

/// <summary>
/// DTO que representa un archivo de usuario en formato byte array.
/// </summary>
public class UserFileByteDto
{
    public byte[] File { get; set; }
    public string FileName { get; set; }
}