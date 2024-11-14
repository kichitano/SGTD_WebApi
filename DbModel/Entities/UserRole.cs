using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("UserRoles")]
public class UserRole : Base
{
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role Role { get; set; }
}