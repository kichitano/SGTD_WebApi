namespace SGTD_WebApi.Models.UserPosition;

/// <summary>
/// DTO que representa la asignación de un usuario a una posición organizacional.
/// </summary>
public class UserPositionDto
{
    public int Id { get; set; }
    public Guid UserGuid { get; set; }
    public int PositionId { get; set; }
}