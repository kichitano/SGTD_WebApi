using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.PositionDependency;

/// <summary>
/// Parámetros de solicitud para operaciones de dependencias entre posiciones.
/// </summary>
public class PositionDependencyRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int ParentPositionId { get; set; }

    [Required]
    public int ChildPositionId { get; set; }
}