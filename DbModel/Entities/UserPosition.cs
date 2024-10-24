using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("UserPositions")]
public class UserPosition : Base
{
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public int PositionId { get; set; }

    [ForeignKey("PositionId")]
    public Position Position { get; set; }
}