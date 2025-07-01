using SGTD_WebApi.DbModels.Enums;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Registro de auditoría del sistema para el seguimiento de cambios
/// </summary>
public class LogSystem
{
    /// <summary>
    /// Identificador único del registro de log
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Nombre de la entidad que fue modificada
    /// </summary>
    public string EntityName { get; set; }
    /// <summary>
    /// Tipo de acción realizada
    /// </summary>
    public ActionTypeEnum Action { get; set; }
    /// <summary>
    /// Valor anterior antes del cambio
    /// </summary>
    public string? PreviousValue { get; set; }
    /// <summary>
    /// Nuevo valor después del cambio
    /// </summary>
    public string? NewValue { get; set; }
    /// <summary>
    /// Fecha y hora del registro
    /// </summary>
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// Identificador del usuario que realizó la acción
    /// </summary>
    public Guid UserId { get; set; }
}