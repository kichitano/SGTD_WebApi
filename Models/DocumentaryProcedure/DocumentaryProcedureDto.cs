namespace SGTD_WebApi.Models.DocumentaryProcedure;

/// <summary>
/// DTO que representa un procedimiento documental del sistema
/// </summary>
public class DocumentaryProcedureDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Status { get; set; }
    public int AreaId { get; set; }
    public string AreaName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DocumentaryProcedureStepDto> Steps { get; set; } = new List<DocumentaryProcedureStepDto>();
}

/// <summary>
/// DTO que representa un paso de un procedimiento documental
/// </summary>
public class DocumentaryProcedureStepDto
{
    public int Id { get; set; }
    public int DocumentaryProcedureId { get; set; }
    public int AreaId { get; set; }
    public string AreaName { get; set; }
    public int PositionId { get; set; }
    public string PositionName { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DocumentaryProcedureStepDocumentDto> Documents { get; set; } = new List<DocumentaryProcedureStepDocumentDto>();
}

/// <summary>
/// DTO que representa un documento requerido en un paso del procedimiento
/// </summary>
public class DocumentaryProcedureStepDocumentDto
{
    public int Id { get; set; }
    public int DocumentaryProcedureStepId { get; set; }
    public int DocumentTypeId { get; set; }
    public string DocumentTypeName { get; set; }
    public bool IsUploadable { get; set; }
    public bool RequiresSignature { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}