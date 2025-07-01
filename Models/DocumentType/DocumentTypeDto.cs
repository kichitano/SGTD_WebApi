namespace SGTD_WebApi.Models.DocumentType;

/// <summary>
/// DTO que representa un tipo de documento en el sistema
/// </summary>
public class DocumentTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsUploadable { get; set; }
}