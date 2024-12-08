namespace SGTD_WebApi.Models.User;
using DbModel.Entities;

public class UserDto
{
    public int? Id { get; set; }
    public Guid UserGuid { get; set; }
    public int PersonId { get; set; }
    public int? PositionId { get; set; }
    public string? PositionName { get; set; }
    public string Email { get; set; }
    public long StorageSize { get; set; }
    public bool Status { get; set; }
    public Person? Person { get; set; }
}