using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un componente del sistema para control de permisos
/// </summary>
[Table("Components")]
public class Component : Base
{
    /// <summary>
    /// Nombre del componente del sistema
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}