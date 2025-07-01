using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un tipo de documento utilizado en los procedimientos documentales
/// </summary>
[Table("DocumentTypes")]
public class DocumentType : Base
{
    /// <summary>
    /// Nombre del tipo de documento
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Indica si este tipo de documento puede ser cargado por los usuarios
    /// </summary>
    public bool IsUploadable { get; set; }
}