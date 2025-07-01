using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Entidad que establece la relación entre usuarios y roles del sistema
/// </summary>
[Table("UserRoles")]
public class UserRole : Base
{
    /// <summary>
    /// Identificador del usuario al que se le asigna el rol
    /// </summary>
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    /// <summary>
    /// Identificador del rol asignado al usuario
    /// </summary>
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}