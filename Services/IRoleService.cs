using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services;

public interface IRoleService
{
    Task CreateAsync(RoleRequestParams requestParams);
    Task UpdateAsync(RoleRequestParams requestParams);
    Task<List<RoleDto>> GetAllAsync();
    Task<RoleDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);

    Task<int> CreateReturnIdAsync(RoleRequestParams requestParams);
}