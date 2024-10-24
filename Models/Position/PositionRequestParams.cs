using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.Position;

public class PositionRequestParams
{
    public int? Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    [Required]
    public int AreaId { get; set; }

    public int Hierarchy { get; set; }
}   