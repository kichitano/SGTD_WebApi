namespace SGTD_WebApi.Models.RoleComponentPermission;

public class RoleComponentPermissionDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int ComponentId { get; set; }
    public int PermissionId { get; set; }
}