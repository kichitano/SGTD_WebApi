using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.RolePermission;

public class RolePermissionRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int RoleId { get; set; }

    [Required]
    public int PermissionId { get; set; }
}