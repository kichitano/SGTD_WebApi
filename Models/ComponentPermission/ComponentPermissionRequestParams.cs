using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.ComponentPermission;

/// <summary>
/// Parámetros de solicitud para operaciones de permisos de componentes
/// </summary>
public class ComponentPermissionRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int ComponentId { get; set; }

    [Required]
    public int PermissionId { get; set; }
}