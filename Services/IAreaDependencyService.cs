using SGTD_WebApi.Models.AreaDependency;

namespace SGTD_WebApi.Services;

public interface IAreaDependencyService
{
    Task CreateAsync(AreaDependencyRequestParams requestParams);
    Task UpdateAsync(AreaDependencyRequestParams requestParams);
    Task<List<AreaDependencyDto>> GetAllAsync();
    Task<AreaDependencyDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}