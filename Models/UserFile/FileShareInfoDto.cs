namespace SGTD_WebApi.Models.UserFile;

public class FileShareInfoDto
{
    public int FileId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public List<FileShareUserDto> SharedUsers { get; set; } = new List<FileShareUserDto>();
}

public class FileShareUserDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
    public string SharedByName { get; set; } = string.Empty;
}