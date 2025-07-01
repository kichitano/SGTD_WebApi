using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un área organizacional dentro del sistema
/// </summary>
[Table("Areas")]
public class Area : Base
{
    /// <summary>
    /// Nombre del área
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    /// <summary>
    /// Descripción detallada del área
    /// </summary>
    [StringLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// Estado activo/inactivo del área
    /// </summary>
    public bool Status { get; set; }
}