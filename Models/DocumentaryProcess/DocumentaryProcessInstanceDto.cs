using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.DocumentaryProcedure;
using SGTD_WebApi.Models.User;

namespace SGTD_WebApi.Models.DocumentaryProcess;

public class DocumentaryProcessInstanceDto
{
    public int Id { get; set; }
    public required string ProcessNumber { get; set; }
    public int DocumentaryProcedureId { get; set; }
    public required string DocumentaryProcedureName { get; set; }
    public int RequestedByUserId { get; set; }
    public required string RequestedByUserName { get; set; }
    public required string RequestedByUserEmail { get; set; }
    public int CurrentStepOrder { get; set; }
    public DocumentaryProcessStatus Status { get; set; }
    public required string StatusName { get; set; }
    public string? Notes { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<DocumentaryProcessStepInstanceDto> StepInstances { get; set; } = new();
    public List<DocumentaryProcessDocumentDto> Documents { get; set; } = new();
    public List<DocumentaryProcessDocumentDto> RequiredUploadDocuments { get; set; } = new();
    public List<DocumentaryProcessDocumentDto> AvailableDownloadDocuments { get; set; } = new();
}

public class CreateDocumentaryProcessInstanceDto
{
    public int DocumentaryProcedureId { get; set; }
    public string? Notes { get; set; }
    public List<UploadDocumentDto> UploadDocuments { get; set; } = new();
}

public class UpdateProcessStepDto
{
    public int ProcessStepInstanceId { get; set; }
    public DocumentaryStepStatus Status { get; set; }
    public string? Notes { get; set; }
    public List<UploadDocumentDto> Documents { get; set; } = new();
}

public class UploadDocumentDto
{
    public int DocumentTypeId { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long FileSize { get; set; }
    public required string FileData { get; set; } // Base64 encoded
    public bool IsSignatureRequired { get; set; } = false;
}