using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

[Table("DocumentaryProcedures")]
public class DocumentaryProcedure : Base
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public bool Status { get; set; }

    public int AreaId { get; set; }

    [ForeignKey("AreaId")]
    public Area Area { get; set; }
}