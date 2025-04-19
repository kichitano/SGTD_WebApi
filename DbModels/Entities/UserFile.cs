using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

[Table("UserFiles")]
public class UserFile : Base
{
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    [Required]
    [StringLength(255)]
    public string FileName { get; set; }
    [Required]
    public long FileSize { get; set; }
    [Required]
    public string ContentType { get; set; }
}