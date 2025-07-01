using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models;

/// <summary>
/// Parámetros de solicitud para operaciones de eliminación por ID
/// </summary>
public class DeleteRequestParams
{
    [Required(ErrorMessage = "El ID es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0.")]
    public int Id { get; set; }
}

/// <summary>
/// Parámetros de solicitud para operaciones de eliminación por GUID
/// </summary>
public class DeleteByGuidRequestParams
{
    [Required(ErrorMessage = "El GUID es requerido.")]
    public Guid Guid { get; set; }
}