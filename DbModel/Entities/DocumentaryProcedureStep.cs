using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModel.Entities;

[Table("DocumentaryProcedureSteps")]
public class DocumentaryProcedureStep : Base
{
    public int DocumentaryProcedureId { get; set; }

    [ForeignKey("DocumentaryProcedureId")]
    public DocumentaryProcedure DocumentaryProcedure { get; set; }

    public int AreaId { get; set; }
    
    [ForeignKey("AreaId")]
    public Area Area { get; set; }

    public int PositionId { get; set; }

    [ForeignKey("PositionId")]
    public Position Position { get; set; }

    [Required]
    public int Order { get; set; }
}