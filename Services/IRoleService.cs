using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services;

public interface IRoleService : IBaseService<RoleRequestParams, RoleDto>
{
    public Task<int> CreateReturnIdAsync(RoleRequestParams requestParams);
}