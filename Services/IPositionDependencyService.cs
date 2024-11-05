using SGTD_WebApi.Models.PositionDependency;

namespace SGTD_WebApi.Services;

public interface IPositionDependencyService
{
    Task CreateAsync(PositionDependencyRequestParams requestParams);
    Task UpdateAsync(PositionDependencyRequestParams requestParams);
    Task<List<PositionDependencyDto>> GetAllAsync();
    Task<PositionDependencyDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}