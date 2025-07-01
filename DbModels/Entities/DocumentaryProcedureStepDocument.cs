using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa la relación entre un paso de procedimiento documental y los tipos de documentos requeridos
/// </summary>
[Table("DocumentaryProcedureStepDocuments")]
public class DocumentaryProcedureStepDocument : Base
{
    /// <summary>
    /// Identificador del paso del procedimiento documental
    /// </summary>
    public int DocumentaryProcedureStepId { get; set; }

    [ForeignKey("DocumentaryProcedureStepId")]
    public DocumentaryProcedureStep DocumentaryProcedureStep { get; set; }

    /// <summary>
    /// Identificador del tipo de documento
    /// </summary>
    public int DocumentTypeId { get; set; }
    
    [ForeignKey("DocumentTypeId")]
    public DocumentType DocumentType { get; set; }
    
    /// <summary>
    /// Indica si el documento requiere firma digital
    /// </summary>
    public bool RequiresSignature { get; set; } = false;
}