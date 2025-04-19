using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

[Table("Areas")]
public class Area : Base
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public bool Status { get; set; }
}