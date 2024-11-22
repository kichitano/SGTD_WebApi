namespace SGTD_WebApi.Models.UserFile;

public class UserFileRequestParams
{
    public Guid UserGuid { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
}