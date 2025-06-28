using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.DocumentaryProcedure;

public class DocumentaryProcedureRequestParams
{
    public int? Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public bool Status { get; set; } = true;

    [Required]
    public int AreaId { get; set; }

    public List<DocumentaryProcedureStepRequestParams> Steps { get; set; } = new List<DocumentaryProcedureStepRequestParams>();
}

public class DocumentaryProcedureStepRequestParams
{
    public int? Id { get; set; }
    public int? DocumentaryProcedureId { get; set; }

    [Required]
    public int AreaId { get; set; }

    [Required]
    public int PositionId { get; set; }

    [Required]
    public int Order { get; set; }

    public List<DocumentaryProcedureStepDocumentRequestParams> Documents { get; set; } = new List<DocumentaryProcedureStepDocumentRequestParams>();
}

public class DocumentaryProcedureStepDocumentRequestParams
{
    public int? Id { get; set; }
    public int? DocumentaryProcedureStepId { get; set; }

    [Required]
    public int DocumentTypeId { get; set; }
    
    public bool RequiresSignature { get; set; } = false;
}