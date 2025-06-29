namespace SGTD_WebApi.Models.DocumentaryProcess;

public class DocumentaryProcessDocumentDto
{
    public int Id { get; set; }
    public int DocumentaryProcessInstanceId { get; set; }
    public int? DocumentaryProcessStepInstanceId { get; set; }
    public int DocumentTypeId { get; set; }
    public required string DocumentTypeName { get; set; }
    public bool DocumentTypeIsUploadable { get; set; }
    public int UploadedByUserId { get; set; }
    public required string UploadedByUserName { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required string ContentType { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsSignatureRequired { get; set; }
    public bool IsSigned { get; set; }
    public bool CanDownload { get; set; }
    public int? StepOrder { get; set; }
}