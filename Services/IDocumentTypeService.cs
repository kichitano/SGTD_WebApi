using SGTD_WebApi.Models.DocumentType;

namespace SGTD_WebApi.Services;

public interface IDocumentTypeService
{
    Task CreateAsync(DocumentTypeRequestParams requestParams);
    Task UpdateAsync(DocumentTypeRequestParams requestParams);
    Task<List<DocumentTypeDto>> GetAllAsync();
    Task<DocumentTypeDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}