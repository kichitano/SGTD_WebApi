using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

[Table("UserFileShares")]
public class UserFileShare : Base
{
    public int UserFileId { get; set; }
    [ForeignKey("UserFileId")]
    public UserFile UserFile { get; set; }

    public int SharedWithUserId { get; set; }
    [ForeignKey("SharedWithUserId")]
    public User SharedWithUser { get; set; }

    public int SharedByUserId { get; set; }
    [ForeignKey("SharedByUserId")]
    public User SharedByUser { get; set; }

    [Required]
    public DateTime SharedAt { get; set; }
}