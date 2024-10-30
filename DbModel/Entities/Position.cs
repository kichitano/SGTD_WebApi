using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("Positions")]
public class Position : Base
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public int AreaId { get; set; }

    [ForeignKey("AreaId")]
    public Area Area { get; set; }
}