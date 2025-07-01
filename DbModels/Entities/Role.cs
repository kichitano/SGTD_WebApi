using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Entidad que representa un rol en el sistema con sus respectivos permisos y características
/// </summary>
[Table("Roles")]
public class Role : Base
{
    /// <summary>
    /// Nombre del rol (único en el sistema)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Descripción detallada del rol y sus responsabilidades
    /// </summary>
    [StringLength(200)]
    public string Description { get; set; }
}