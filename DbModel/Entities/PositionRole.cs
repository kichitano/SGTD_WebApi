using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("PositionRoles")]
public class PositionRole : Base
{
    public int PositionId { get; set; }

    [ForeignKey("PositionId")]
    public Position Position { get; set; }

    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}