using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

[Table("DocumentaryProcessInstances")]
public class DocumentaryProcessInstance : Base
{
    [Required]
    [StringLength(50)]
    public required string ProcessNumber { get; set; }

    public int DocumentaryProcedureId { get; set; }

    [ForeignKey("DocumentaryProcedureId")]
    public DocumentaryProcedure DocumentaryProcedure { get; set; } = null!;

    public int RequestedByUserId { get; set; }

    [ForeignKey("RequestedByUserId")]
    public User RequestedByUser { get; set; } = null!;

    public int CurrentStepOrder { get; set; } = 1;

    public DocumentaryProcessStatus Status { get; set; } = DocumentaryProcessStatus.InProgress;

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public List<DocumentaryProcessStepInstance> StepInstances { get; set; } = new();
    
    public List<DocumentaryProcessDocument> Documents { get; set; } = new();
}

public enum DocumentaryProcessStatus
{
    InProgress = 1,
    Completed = 2,
    Rejected = 3,
    Cancelled = 4
}