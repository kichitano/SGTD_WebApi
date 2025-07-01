namespace SGTD_WebApi.Models.ComponentPermission;

/// <summary>
/// DTO que representa la relación entre un componente y un permiso del sistema
/// </summary>
public class ComponentPermissionDto
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public int PermissionId { get; set; }
}