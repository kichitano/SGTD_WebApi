namespace SGTD_WebApi.Models.RoleComponentPermission;

/// <summary>
/// DTO que representa la asignación de permisos de componentes a roles.
/// </summary>
public class RoleComponentPermissionDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int ComponentId { get; set; }
    public int PermissionId { get; set; }
}