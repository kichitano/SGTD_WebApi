using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.PositionRole;

/// <summary>
/// Parámetros de solicitud para operaciones de asignación de roles a usuarios.
/// </summary>
public class UserRoleRequestParams
{
    public int? Id { get; set; }

    [Required]
    public Guid UserGuid { get; set; }

    [Required]
    public int RoleId { get; set; }
}