using SGTD_WebApi.Models.RoleComponentPermission;

namespace SGTD_WebApi.Services;

public interface IRoleComponentPermissionService : IBaseService<RoleComponentPermissionRequestParams, RoleComponentPermissionDto>
{
    Task CreateArrayAsync(RoleComponentPermissionRequestParams[] requestParams);
    Task UpdateArrayAsync(int roleId, RoleComponentPermissionRequestParams[] requestParams);

    Task<List<RoleComponentPermissionDto>> GetByRoleIdAsync(int roleId);
}