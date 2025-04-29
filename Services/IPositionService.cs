using SGTD_WebApi.Models.Position;
using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services;

public interface IPositionService
{
    Task CreateAsync(PositionRequestParams requestParams);
    Task UpdateAsync(PositionRequestParams requestParams);
    Task<List<PositionDto>> GetAllAsync();
    Task<PositionDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);

    Task<int> CreateReturnIdAsync(PositionRequestParams requestParams);
    Task<List<PositionDto>> GetAllByAreaIdAsync(int areaId);

}