namespace SGTD_WebApi.Models.UserFile;

/// <summary>
/// Parámetros de solicitud para operaciones de archivos de usuario.
/// </summary>
public class UserFileRequestParams
{
    public Guid UserGuid { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
}