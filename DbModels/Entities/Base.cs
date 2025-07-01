using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Clase base abstracta para todas las entidades del sistema con propiedades comunes de auditoría
/// </summary>
public abstract class Base
{
    /// <summary>
    /// Identificador único de la entidad
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Fecha y hora de creación de la entidad
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha y hora de la última actualización de la entidad
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Indica si la entidad está marcada como eliminada (eliminación lógica)
    /// </summary>
    public bool IsDeleted { get; set; } = false;
    
    /// <summary>
    /// Fecha y hora de eliminación lógica de la entidad
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}