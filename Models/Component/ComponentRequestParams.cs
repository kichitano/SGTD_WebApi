using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.Component;

public class ComponentRequestParams
{
    public int? Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}