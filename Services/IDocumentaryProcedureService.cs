using SGTD_WebApi.Models.DocumentaryProcedure;

namespace SGTD_WebApi.Services;

public interface IDocumentaryProcedureService
{
    Task CreateAsync(DocumentaryProcedureRequestParams requestParams);
    Task<List<DocumentaryProcedureDto>> GetAllAsync();
    Task<DocumentaryProcedureDto> GetByIdAsync(int id);
    Task UpdateAsync(DocumentaryProcedureRequestParams requestParams);
    Task DeleteAsync(int id);
}