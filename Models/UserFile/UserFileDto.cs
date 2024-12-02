namespace SGTD_WebApi.Models.UserFile;

public class UserFileDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public decimal FileSize { get; set; }
    public string ContentType { get; set; }
    public DateTime CreatedAt { get; set; }
}