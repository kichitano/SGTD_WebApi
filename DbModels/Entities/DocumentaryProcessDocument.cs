using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModels.Entities;

[Table("DocumentaryProcessDocuments")]
public class DocumentaryProcessDocument : Base
{
    public int DocumentaryProcessInstanceId { get; set; }

    [ForeignKey("DocumentaryProcessInstanceId")]
    public DocumentaryProcessInstance DocumentaryProcessInstance { get; set; } = null!;

    public int? DocumentaryProcessStepInstanceId { get; set; }

    [ForeignKey("DocumentaryProcessStepInstanceId")]
    public DocumentaryProcessStepInstance? DocumentaryProcessStepInstance { get; set; }

    public int DocumentTypeId { get; set; }

    [ForeignKey("DocumentTypeId")]
    public DocumentType DocumentType { get; set; } = null!;

    public int UploadedByUserId { get; set; }

    [ForeignKey("UploadedByUserId")]
    public User UploadedByUser { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public required string FileName { get; set; }

    [Required]
    [StringLength(255)]
    public required string FilePath { get; set; }

    [Required]
    public required string ContentType { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public bool IsSignatureRequired { get; set; } = false;

    public bool IsSigned { get; set; } = false;
}