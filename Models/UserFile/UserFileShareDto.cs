namespace SGTD_WebApi.Models.UserFile;

/// <summary>
/// DTO que representa un archivo compartido entre usuarios.
/// </summary>
public class UserFileShareDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string SharedByName { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
    public bool IsOwner { get; set; }
}