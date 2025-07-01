namespace SGTD_WebApi.Models.UserFile;

/// <summary>
/// DTO que representa un archivo de usuario en el sistema.
/// </summary>
public class UserFileDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public decimal FileSize { get; set; }
    public string ContentType { get; set; }
    public DateTime CreatedAt { get; set; }
}