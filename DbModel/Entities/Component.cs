using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("Components")]
public class Component : Base
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}