using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.User;

/// <summary>
/// Parámetros de solicitud para eliminación de usuarios.
/// </summary>
public class UserDeletedRequestParams
{
    [Required]
    public Guid? UserGuid { get; set; }
}