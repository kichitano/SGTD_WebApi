namespace SGTD_WebApi.Models.UserFile;

/// <summary>
/// Parámetros para revocar el acceso compartido de un archivo.
/// </summary>
public class UnshareFileRequestParams
{
    public int FileId { get; set; }
    public int UserId { get; set; }
    public Guid UserGuid { get; set; }
}