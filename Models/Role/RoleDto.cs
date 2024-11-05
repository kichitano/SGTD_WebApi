namespace SGTD_WebApi.Models.Role;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? PermissionCount { get; set; }
}