using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un documento asociado a una instancia de proceso documental
/// </summary>
[Table("DocumentaryProcessDocuments")]
public class DocumentaryProcessDocument : Base
{
    /// <summary>
    /// Identificador de la instancia del proceso documental
    /// </summary>
    public int DocumentaryProcessInstanceId { get; set; }

    [ForeignKey("DocumentaryProcessInstanceId")]
    public DocumentaryProcessInstance DocumentaryProcessInstance { get; set; } = null!;

    /// <summary>
    /// Identificador de la instancia del paso del proceso (opcional)
    /// </summary>
    public int? DocumentaryProcessStepInstanceId { get; set; }

    [ForeignKey("DocumentaryProcessStepInstanceId")]
    public DocumentaryProcessStepInstance? DocumentaryProcessStepInstance { get; set; }

    /// <summary>
    /// Identificador del tipo de documento
    /// </summary>
    public int DocumentTypeId { get; set; }

    [ForeignKey("DocumentTypeId")]
    public DocumentType DocumentType { get; set; } = null!;

    /// <summary>
    /// Identificador del usuario que subió el documento
    /// </summary>
    public int UploadedByUserId { get; set; }

    [ForeignKey("UploadedByUserId")]
    public User UploadedByUser { get; set; } = null!;

    /// <summary>
    /// Nombre del archivo
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string FileName { get; set; }

    /// <summary>
    /// Ruta donde se almacena el archivo
    /// </summary>
    [Required]
    [StringLength(255)]
    public required string FilePath { get; set; }

    /// <summary>
    /// Tipo de contenido MIME del archivo
    /// </summary>
    [Required]
    public required string ContentType { get; set; }

    /// <summary>
    /// Tamaño del archivo en bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Fecha y hora de carga del documento
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica si el documento requiere firma
    /// </summary>
    public bool IsSignatureRequired { get; set; } = false;

    /// <summary>
    /// Indica si el documento ha sido firmado
    /// </summary>
    public bool IsSigned { get; set; } = false;
}