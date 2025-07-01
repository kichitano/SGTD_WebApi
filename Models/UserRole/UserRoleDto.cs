namespace SGTD_WebApi.Models.PositionRole;

/// <summary>
/// DTO que representa la asignación de un rol a un usuario.
/// </summary>
public class UserRoleDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
}