using SGTD_WebApi.Models.Area;

namespace SGTD_WebApi.Services;

public interface IAreaService
{
    Task CreateAsync(AreaRequestParams requestParams);
    Task UpdateAsync(AreaRequestParams requestParams);
    Task<List<AreaDto>> GetAllAsync();
    Task<AreaDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}