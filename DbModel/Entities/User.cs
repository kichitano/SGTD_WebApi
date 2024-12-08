using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("Users")]
public class User : Base
{
    public Guid UserGuid { get; set; }
    public int PersonId { get; set; }

    [ForeignKey("PersonId")]
    public Person Person { get; set; }

    public int? PositionId { get; set; }

    [ForeignKey("PositionId")]
    public Position? Position { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    [Required]
    public long StorageSize { get; set; }

    [Required]
    public string FolderPath { get; set; }

    public bool Status { get; set; }
}