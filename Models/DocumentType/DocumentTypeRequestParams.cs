using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.DocumentType;

public class DocumentTypeRequestParams
{
    public int? Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public bool IsUploadable { get; set; }
}