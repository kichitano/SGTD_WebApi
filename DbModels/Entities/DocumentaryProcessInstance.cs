using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa una instancia específica de un proceso documental en ejecución
/// </summary>
[Table("DocumentaryProcessInstances")]
public class DocumentaryProcessInstance : Base
{
    /// <summary>
    /// Número único del proceso
    /// </summary>
    [Required]
    [StringLength(50)]
    public required string ProcessNumber { get; set; }

    /// <summary>
    /// Identificador del procedimiento documental asociado
    /// </summary>
    public int DocumentaryProcedureId { get; set; }

    [ForeignKey("DocumentaryProcedureId")]
    public DocumentaryProcedure DocumentaryProcedure { get; set; } = null!;

    /// <summary>
    /// Identificador del usuario que solicitó el proceso
    /// </summary>
    public int RequestedByUserId { get; set; }

    [ForeignKey("RequestedByUserId")]
    public User RequestedByUser { get; set; } = null!;

    /// <summary>
    /// Orden del paso actual en el proceso
    /// </summary>
    public int CurrentStepOrder { get; set; } = 1;

    /// <summary>
    /// Estado actual del proceso
    /// </summary>
    public DocumentaryProcessStatus Status { get; set; } = DocumentaryProcessStatus.InProgress;

    /// <summary>
    /// Notas adicionales del proceso
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Fecha y hora de solicitud del proceso
    /// </summary>
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora de finalización del proceso
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Instancias de los pasos del proceso
    /// </summary>
    public List<DocumentaryProcessStepInstance> StepInstances { get; set; } = new();
    
    /// <summary>
    /// Documentos asociados al proceso
    /// </summary>
    public List<DocumentaryProcessDocument> Documents { get; set; } = new();
}

/// <summary>
/// Estados posibles de un proceso documental
/// </summary>
public enum DocumentaryProcessStatus
{
    /// <summary>
    /// Proceso en curso
    /// </summary>
    InProgress = 1,
    /// <summary>
    /// Proceso completado exitosamente
    /// </summary>
    Completed = 2,
    /// <summary>
    /// Proceso rechazado
    /// </summary>
    Rejected = 3,
    /// <summary>
    /// Proceso cancelado
    /// </summary>
    Cancelled = 4
}