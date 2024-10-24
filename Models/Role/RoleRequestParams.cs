using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.Role;

public class RoleRequestParams
{
    public int? Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }
}