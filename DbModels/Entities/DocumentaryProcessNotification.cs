using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

[Table("DocumentaryProcessNotifications")]
public class DocumentaryProcessNotification : Base
{
    public int DocumentaryProcessInstanceId { get; set; }

    [ForeignKey("DocumentaryProcessInstanceId")]
    public DocumentaryProcessInstance DocumentaryProcessInstance { get; set; } = null!;

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public required string Title { get; set; }

    [Required]
    [StringLength(500)]
    public required string Message { get; set; }

    public DocumentaryNotificationType Type { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReadAt { get; set; }
}

public enum DocumentaryNotificationType
{
    ProcessStarted = 1,
    StepAssigned = 2,
    StepCompleted = 3,
    ProcessCompleted = 4,
    ProcessRejected = 5,
    DocumentRequired = 6
}