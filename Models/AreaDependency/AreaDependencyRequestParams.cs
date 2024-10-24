using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.AreaDependency;

public class AreaDependencyRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int ParentAreaId { get; set; }

    [Required]
    public int ChildAreaId { get; set; }
}