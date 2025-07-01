using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.AreaDependency;

/// <summary>
/// Parámetros de solicitud para operaciones con dependencias entre áreas
/// </summary>
public class AreaDependencyRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int ParentAreaId { get; set; }

    [Required]
    public int ChildAreaId { get; set; }
}