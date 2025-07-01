using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un permiso del sistema para control de acceso
/// </summary>
[Table("Permissions")]
public class Permission : Base
{
    /// <summary>
    /// Nombre del permiso
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}