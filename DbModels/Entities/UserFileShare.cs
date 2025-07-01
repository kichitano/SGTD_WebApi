using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Entidad que gestiona el compartir archivos entre usuarios del sistema
/// </summary>
[Table("UserFileShares")]
public class UserFileShare : Base
{
    /// <summary>
    /// Identificador del archivo que se está compartiendo
    /// </summary>
    public int UserFileId { get; set; }
    [ForeignKey("UserFileId")]
    public UserFile UserFile { get; set; }

    /// <summary>
    /// Identificador del usuario con quien se comparte el archivo
    /// </summary>
    public int SharedWithUserId { get; set; }
    [ForeignKey("SharedWithUserId")]
    public User SharedWithUser { get; set; }

    /// <summary>
    /// Identificador del usuario que comparte el archivo
    /// </summary>
    public int SharedByUserId { get; set; }
    [ForeignKey("SharedByUserId")]
    public User SharedByUser { get; set; }

    /// <summary>
    /// Fecha y hora en que se compartió el archivo
    /// </summary>
    [Required]
    public DateTime SharedAt { get; set; }
}