namespace SGTD_WebApi.Models.UserPosition;

public class UserPositionDto
{
    public int Id { get; set; }
    public Guid UserGuid { get; set; }
    public int PositionId { get; set; }
}