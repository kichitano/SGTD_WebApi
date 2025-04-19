using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

[Table("DocumentTypes")]
public class DocumentType : Base
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    public bool IsUploadable { get; set; }
}