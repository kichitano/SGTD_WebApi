namespace SGTD_WebApi.Models.UserFile;

public class DeleteMultipleFilesRequestDto
{
    public List<int> FileIds { get; set; } = new List<int>();
    public Guid UserGuid { get; set; }
}