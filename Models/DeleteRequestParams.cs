using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models;

public class DeleteRequestParams
{
    [Required(ErrorMessage = "El ID es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0.")]
    public int Id { get; set; }
}

public class DeleteByGuidRequestParams
{
    [Required(ErrorMessage = "El GUID es requerido.")]
    public Guid Guid { get; set; }
}