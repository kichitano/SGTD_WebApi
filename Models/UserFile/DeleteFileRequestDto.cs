namespace SGTD_WebApi.Models.UserFile;

public class DeleteFileRequestDto
{
    public int FileId { get; set; }
    public Guid UserGuid { get; set; }
}