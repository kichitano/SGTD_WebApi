using SGTD_WebApi.Models.UserPosition;

namespace SGTD_WebApi.Services;

public interface IUserPositionService
{
    Task CreateAsync(UserPositionRequestParams requestParams);
    Task UpdateAsync(UserPositionRequestParams requestParams);
    Task<List<UserPositionDto>> GetAllAsync();
    Task<UserPositionDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}