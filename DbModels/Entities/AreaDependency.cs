using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

[Table("AreaDependencies")]
public class AreaDependency : Base
{
    public int ParentAreaId { get; set; }

    [ForeignKey("ParentAreaId")]
    public Area ParentArea { get; set; }

    public int ChildAreaId { get; set; }

    [ForeignKey("ChildAreaId")]
    public Area ChildArea { get; set; }
}