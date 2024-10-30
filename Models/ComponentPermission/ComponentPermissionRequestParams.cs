using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.ComponentPermission;

public class ComponentPermissionRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int ComponentId { get; set; }

    [Required]
    public int PermissionId { get; set; }
}