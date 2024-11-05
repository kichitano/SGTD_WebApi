using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("PositionDependencies")]
public class PositionDependency : Base
{
    public int ParentPositionId { get; set; }

    [ForeignKey("ParentPositionId")]
    public Position ParentPosition { get; set; }

    public int ChildPositionId { get; set; }

    [ForeignKey("ChildPositionId")]
    public Position ChildPosition { get; set; }
}