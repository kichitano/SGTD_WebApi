using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Entidad que define los permisos específicos que tiene un rol sobre un componente del sistema
/// </summary>
[Table("RoleComponentPermissions")]
public class RoleComponentPermission : Base
{
    /// <summary>
    /// Identificador del rol al que se le asigna el permiso
    /// </summary>
    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public Role Role { get; set; }

    /// <summary>
    /// Identificador del componente sobre el cual se aplica el permiso
    /// </summary>
    public int ComponentId { get; set; }
    [ForeignKey("ComponentId")]
    public Component Component { get; set; }

    /// <summary>
    /// Identificador del permiso específico otorgado
    /// </summary>
    public int PermissionId { get; set; }
    [ForeignKey("PermissionId")]
    public Permission Permission { get; set; }
}