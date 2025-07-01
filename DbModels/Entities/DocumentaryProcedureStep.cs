using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un paso específico dentro de un procedimiento documental
/// </summary>
[Table("DocumentaryProcedureSteps")]
public class DocumentaryProcedureStep : Base
{
    /// <summary>
    /// Identificador del procedimiento documental al que pertenece este paso
    /// </summary>
    public int DocumentaryProcedureId { get; set; }

    /// <summary>
    /// Procedimiento documental al que pertenece este paso
    /// </summary>
    [ForeignKey("DocumentaryProcedureId")]
    public DocumentaryProcedure DocumentaryProcedure { get; set; }

    /// <summary>
    /// Identificador del área responsable de ejecutar este paso
    /// </summary>
    public int AreaId { get; set; }
    
    /// <summary>
    /// Área responsable de ejecutar este paso del procedimiento
    /// </summary>
    [ForeignKey("AreaId")]
    public Area Area { get; set; }

    /// <summary>
    /// Identificador del puesto responsable de ejecutar este paso
    /// </summary>
    public int PositionId { get; set; }

    /// <summary>
    /// Puesto responsable de ejecutar este paso del procedimiento
    /// </summary>
    [ForeignKey("PositionId")]
    public Position Position { get; set; }

    /// <summary>
    /// Orden secuencial de ejecución del paso dentro del procedimiento
    /// </summary>
    [Required]
    public int Order { get; set; }
}