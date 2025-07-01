namespace SGTD_WebApi.Models.UserFile;

/// <summary>
/// DTO para solicitudes de eliminación de archivos de usuario.
/// </summary>
public class DeleteFileRequestDto
{
    public int FileId { get; set; }
    public Guid UserGuid { get; set; }
}