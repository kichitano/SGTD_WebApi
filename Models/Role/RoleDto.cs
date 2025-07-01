namespace SGTD_WebApi.Models.Role;

/// <summary>
/// DTO que representa un rol del sistema.
/// </summary>
public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? PermissionCount { get; set; }
}