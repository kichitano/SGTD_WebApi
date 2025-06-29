using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

[Table("DocumentaryProcessStepInstances")]
public class DocumentaryProcessStepInstance : Base
{
    public int DocumentaryProcessInstanceId { get; set; }

    [ForeignKey("DocumentaryProcessInstanceId")]
    public DocumentaryProcessInstance DocumentaryProcessInstance { get; set; } = null!;

    public int DocumentaryProcedureStepId { get; set; }

    [ForeignKey("DocumentaryProcedureStepId")]
    public DocumentaryProcedureStep DocumentaryProcedureStep { get; set; } = null!;

    public int? AssignedToUserId { get; set; }

    [ForeignKey("AssignedToUserId")]
    public User? AssignedToUser { get; set; }

    public DocumentaryStepStatus Status { get; set; } = DocumentaryStepStatus.Pending;

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public List<DocumentaryProcessDocument> Documents { get; set; } = new();
}

public enum DocumentaryStepStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Rejected = 4
}