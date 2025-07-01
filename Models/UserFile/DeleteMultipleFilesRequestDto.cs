namespace SGTD_WebApi.Models.UserFile;

/// <summary>
/// DTO para solicitudes de eliminación múltiple de archivos de usuario.
/// </summary>
public class DeleteMultipleFilesRequestDto
{
    public List<int> FileIds { get; set; } = new List<int>();
    public Guid UserGuid { get; set; }
}