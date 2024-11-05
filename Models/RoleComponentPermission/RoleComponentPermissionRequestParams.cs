using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.RoleComponentPermission;

public class RoleComponentPermissionRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int RoleId { get; set; }

    [Required]
    public int ComponentId { get; set; }
    [Required]
    public int PermissionId { get; set; }
}