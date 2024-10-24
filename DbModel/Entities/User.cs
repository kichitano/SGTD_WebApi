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

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    public bool Status { get; set; }
}