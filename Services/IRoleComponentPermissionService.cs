using SGTD_WebApi.Models.RoleComponentPermission;

namespace SGTD_WebApi.Services;

public interface IRoleComponentPermissionService
{
    Task CreateAsync(RoleComponentPermissionRequestParams requestParams);
    Task UpdateAsync(RoleComponentPermissionRequestParams requestParams);
    Task<List<RoleComponentPermissionDto>> GetAllAsync();
    Task<RoleComponentPermissionDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);

    Task CreateArrayAsync(RoleComponentPermissionRequestParams[] requestParams);
    Task UpdateArrayAsync(int roleId, RoleComponentPermissionRequestParams[] requestParams);
    Task<List<RoleComponentPermissionDto>> GetByRoleIdAsync(int roleId);
}