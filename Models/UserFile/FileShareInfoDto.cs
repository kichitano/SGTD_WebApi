namespace SGTD_WebApi.Models.UserFile;

/// <summary>
/// DTO que contiene información sobre el estado de compartición de un archivo.
/// </summary>
public class FileShareInfoDto
{
    public int FileId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public List<FileShareUserDto> SharedUsers { get; set; } = new List<FileShareUserDto>();
}

/// <summary>
/// DTO que representa un usuario con quien se ha compartido un archivo.
/// </summary>
public class FileShareUserDto
{
    public int PersonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; }
    public string SharedByName { get; set; } = string.Empty;
}