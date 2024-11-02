using SGTD_WebApi.Models.Component;

namespace SGTD_WebApi.Services;

public interface IComponentService
{
    Task CreateAsync(ComponentRequestParams requestParams);
    Task UpdateAsync(ComponentRequestParams requestParams);
    Task<List<ComponentDto>> GetAllAsync();
    Task<ComponentDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}