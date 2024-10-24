using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.User;

public class UserRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int PersonId { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    public bool Status { get; set; }
}