using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.User;

public class UserDeletedRequestParams
{
    [Required]
    public Guid? UserGuid { get; set; }
}