using SGTD_WebApi.DbModels.Entities;

namespace SGTD_WebApi.Models.DocumentaryProcess;

/// <summary>
/// DTO que representa una instancia de paso en el proceso documental
/// </summary>
public class DocumentaryProcessStepInstanceDto
{
    public int Id { get; set; }
    public int DocumentaryProcessInstanceId { get; set; }
    public int DocumentaryProcedureStepId { get; set; }
    public int StepOrder { get; set; }
    public required string AreaName { get; set; }
    public required string PositionName { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public string? AssignedToUserEmail { get; set; }
    public DocumentaryStepStatus Status { get; set; }
    public required string StatusName { get; set; }
    public string? Notes { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<DocumentaryProcessDocumentDto> Documents { get; set; } = new();
    public List<RequiredDocumentDto> RequiredDocuments { get; set; } = new();
    public bool CanTakeAction { get; set; }
    public bool IsCurrentStep { get; set; }
}

/// <summary>
/// DTO que representa un documento requerido en un paso del proceso
/// </summary>
public class RequiredDocumentDto
{
    public int DocumentTypeId { get; set; }
    public required string DocumentTypeName { get; set; }
    public bool IsUploadable { get; set; }
    public bool RequiresSignature { get; set; }
    public bool IsUploaded { get; set; }
}