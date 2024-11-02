namespace SGTD_WebApi.Models.User;
using SGTD_WebApi.DbModel.Entities;

public class UserDto
{
    public int? Id { get; set; }
    public Guid UserGuid { get; set; }
    public int PersonId { get; set; }
    public string Email { get; set; }
    public long StorageSize { get; set; }
    public bool Status { get; set; }
    public Person? Person { get; set; }
    public Position? Position { get; set; }
}