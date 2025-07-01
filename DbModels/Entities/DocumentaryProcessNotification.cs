using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa una notificación relacionada con procesos documentales
/// </summary>
[Table("DocumentaryProcessNotifications")]
public class DocumentaryProcessNotification : Base
{
    /// <summary>
    /// Identificador de la instancia del proceso documental
    /// </summary>
    public int DocumentaryProcessInstanceId { get; set; }

    [ForeignKey("DocumentaryProcessInstanceId")]
    public DocumentaryProcessInstance DocumentaryProcessInstance { get; set; } = null!;

    /// <summary>
    /// Identificador del usuario destinatario de la notificación
    /// </summary>
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    /// <summary>
    /// Título de la notificación
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string Title { get; set; }

    /// <summary>
    /// Mensaje de la notificación
    /// </summary>
    [Required]
    [StringLength(500)]
    public required string Message { get; set; }

    /// <summary>
    /// Tipo de notificación
    /// </summary>
    public DocumentaryNotificationType Type { get; set; }

    /// <summary>
    /// Indica si la notificación ha sido leída
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// Fecha y hora de envío de la notificación
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora de lectura de la notificación
    /// </summary>
    public DateTime? ReadAt { get; set; }
}

/// <summary>
/// Tipos de notificaciones para procesos documentales
/// </summary>
public enum DocumentaryNotificationType
{
    /// <summary>
    /// Proceso iniciado
    /// </summary>
    ProcessStarted = 1,
    /// <summary>
    /// Paso asignado a usuario
    /// </summary>
    StepAssigned = 2,
    /// <summary>
    /// Paso completado
    /// </summary>
    StepCompleted = 3,
    /// <summary>
    /// Proceso completado
    /// </summary>
    ProcessCompleted = 4,
    /// <summary>
    /// Proceso rechazado
    /// </summary>
    ProcessRejected = 5,
    /// <summary>
    /// Documento requerido
    /// </summary>
    DocumentRequired = 6
}