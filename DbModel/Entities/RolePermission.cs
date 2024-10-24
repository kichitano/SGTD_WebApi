using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("RolePermissions")]
public class RolePermission : Base
{
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }

    public int PermissionId { get; set; }

    [ForeignKey("PermissionId")]
    public Permission Permission { get; set; }
}