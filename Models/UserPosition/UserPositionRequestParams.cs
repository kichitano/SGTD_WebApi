using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.UserPosition;

/// <summary>
/// Parámetros de solicitud para operaciones de asignación usuario-posición.
/// </summary>
public class UserPositionRequestParams
{
    [Required]
    public Guid UserGuid { get; set; }

    [Required]
    public int PositionId { get; set; }
}