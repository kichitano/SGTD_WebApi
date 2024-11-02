using SGTD_WebApi.Models.PositionRole;

namespace SGTD_WebApi.Services;

public interface IPositionRoleService
{
    Task CreateAsync(PositionRoleRequestParams requestParams);
    Task UpdateAsync(PositionRoleRequestParams requestParams);
    Task<List<PositionRoleDto>> GetAllAsync();
    Task<PositionRoleDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);

    Task<List<PositionRoleDto>> GetByPositionIdAsync(int positionId);
}