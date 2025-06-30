namespace SGTD_WebApi.Models.UserFile;

public class UnshareFileRequestParams
{
    public int FileId { get; set; }
    public int UserId { get; set; }
    public Guid UserGuid { get; set; }
}