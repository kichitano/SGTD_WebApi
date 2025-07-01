using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un puesto de trabajo en la estructura organizacional
/// </summary>
[Table("Positions")]
public class Position : Base
{
    /// <summary>
    /// Nombre del puesto
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Descripción del puesto
    /// </summary>
    [StringLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// Identificador del área a la que pertenece el puesto
    /// </summary>
    public int AreaId { get; set; }

    [ForeignKey("AreaId")]
    public Area Area { get; set; }

    /// <summary>
    /// Identificador del puesto del jefe directo (opcional)
    /// </summary>
    public int? DirectManagerPositionId { get; set; }

    [ForeignKey("DirectManagerPositionId")]
    public Position? DirectManagerPosition { get; set; }
}