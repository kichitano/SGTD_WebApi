using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

[Table("Permissions")]
public class Permission : Base
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}