using SGTD_WebApi.Models.Permission;

namespace SGTD_WebApi.Services;

public interface IPermissionService
{
    Task CreateAsync(PermissionRequestParams requestParams);
    Task UpdateAsync(PermissionRequestParams requestParams);
    Task<List<PermissionDto>> GetAllAsync();
    Task<PermissionDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}