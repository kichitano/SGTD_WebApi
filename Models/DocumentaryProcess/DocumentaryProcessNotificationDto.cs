using SGTD_WebApi.DbModels.Entities;

namespace SGTD_WebApi.Models.DocumentaryProcess;

/// <summary>
/// DTO que representa una notificación del proceso documental
/// </summary>
public class DocumentaryProcessNotificationDto
{
    public int Id { get; set; }
    public int DocumentaryProcessInstanceId { get; set; }
    public required string ProcessNumber { get; set; }
    public required string DocumentaryProcedureName { get; set; }
    public int UserId { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public DocumentaryNotificationType Type { get; set; }
    public required string TypeName { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

/// <summary>
/// DTO para marcar una notificación como leída
/// </summary>
public class MarkNotificationAsReadDto
{
    public int NotificationId { get; set; }
}