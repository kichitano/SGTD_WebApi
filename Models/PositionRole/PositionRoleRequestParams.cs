using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.PositionRole;

public class PositionRoleRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int PositionId { get; set; }

    [Required]
    public int RoleId { get; set; }
}