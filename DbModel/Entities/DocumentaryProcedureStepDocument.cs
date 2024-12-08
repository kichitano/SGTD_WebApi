using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("DocumentaryProcedureStepDocuments")]
public class DocumentaryProcedureStepDocument : Base
{
    public int DocumentaryProcedureStepId { get; set; }

    [ForeignKey("DocumentaryProcedureStepId")]
    public DocumentaryProcedureStep DocumentaryProcedureStep { get; set; }

    public int DocumentTypeId { get; set; }
    
    [ForeignKey("DocumentTypeId")]
    public DocumentType DocumentType { get; set; }
}