using SGTD_WebApi.Models.PositionRole;

namespace SGTD_WebApi.Services;

public interface IUserRoleService
{
    Task CreateAsync(UserRoleRequestParams requestParams);
    Task UpdateAsync(UserRoleRequestParams requestParams);
    Task<List<UserRoleDto>> GetAllAsync();
    Task<UserRoleDto> GetByIdAsync(int id);
    Task DeleteByUserGuidAsync(Guid userGuid);

    Task<List<UserRoleDto>> GetByUserGuidAsync(Guid userGuid);
}