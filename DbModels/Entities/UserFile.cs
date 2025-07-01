using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Entidad que representa los archivos almacenados por los usuarios en el sistema
/// </summary>
[Table("UserFiles")]
public class UserFile : Base
{
    /// <summary>
    /// Identificador del usuario propietario del archivo
    /// </summary>
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    /// <summary>
    /// Nombre del archivo con su extensión
    /// </summary>
    [Required]
    [StringLength(255)]
    public string FileName { get; set; }
    
    /// <summary>
    /// Tamaño del archivo en bytes
    /// </summary>
    [Required]
    public long FileSize { get; set; }
    
    /// <summary>
    /// Tipo MIME del archivo (application/pdf, image/png, etc.)
    /// </summary>
    [Required]
    public string ContentType { get; set; }
}