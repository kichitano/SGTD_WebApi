using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un procedimiento documental del sistema con sus pasos y configuraciones
/// </summary>
[Table("DocumentaryProcedures")]
public class DocumentaryProcedure : Base
{
    /// <summary>
    /// Nombre del procedimiento documental
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    /// <summary>
    /// Descripción detallada del procedimiento
    /// </summary>
    [StringLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// Estado activo/inactivo del procedimiento
    /// </summary>
    public bool Status { get; set; }

    /// <summary>
    /// Identificador del área responsable del procedimiento
    /// </summary>
    public int AreaId { get; set; }

    /// <summary>
    /// Área responsable del procedimiento documental
    /// </summary>
    [ForeignKey("AreaId")]
    public Area Area { get; set; }
}