using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.UserPosition;

public class UserPositionRequestParams
{
    [Required]
    public Guid UserGuid { get; set; }

    [Required]
    public int PositionId { get; set; }
}