using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.UserPosition;

public class UserPositionRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int PositionId { get; set; }
}