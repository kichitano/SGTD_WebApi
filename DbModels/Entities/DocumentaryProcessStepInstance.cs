using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa una instancia específica de un paso en un proceso documental
/// </summary>
[Table("DocumentaryProcessStepInstances")]
public class DocumentaryProcessStepInstance : Base
{
    /// <summary>
    /// Identificador de la instancia del proceso documental
    /// </summary>
    public int DocumentaryProcessInstanceId { get; set; }

    [ForeignKey("DocumentaryProcessInstanceId")]
    public DocumentaryProcessInstance DocumentaryProcessInstance { get; set; } = null!;

    /// <summary>
    /// Identificador del paso del procedimiento documental
    /// </summary>
    public int DocumentaryProcedureStepId { get; set; }

    [ForeignKey("DocumentaryProcedureStepId")]
    public DocumentaryProcedureStep DocumentaryProcedureStep { get; set; } = null!;

    /// <summary>
    /// Identificador del usuario asignado al paso (opcional)
    /// </summary>
    public int? AssignedToUserId { get; set; }

    [ForeignKey("AssignedToUserId")]
    public User? AssignedToUser { get; set; }

    /// <summary>
    /// Estado actual del paso
    /// </summary>
    public DocumentaryStepStatus Status { get; set; } = DocumentaryStepStatus.Pending;

    /// <summary>
    /// Notas adicionales del paso
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Fecha y hora de inicio del paso
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Fecha y hora de finalización del paso
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Documentos asociados a este paso
    /// </summary>
    public List<DocumentaryProcessDocument> Documents { get; set; } = new();
}

/// <summary>
/// Estados posibles de un paso en un proceso documental
/// </summary>
public enum DocumentaryStepStatus
{
    /// <summary>
    /// Paso pendiente de iniciar
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Paso en progreso
    /// </summary>
    InProgress = 2,
    /// <summary>
    /// Paso completado
    /// </summary>
    Completed = 3,
    /// <summary>
    /// Paso rechazado
    /// </summary>
    Rejected = 4
}