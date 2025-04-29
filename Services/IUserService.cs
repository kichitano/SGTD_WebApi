using SGTD_WebApi.Models.User;

namespace SGTD_WebApi.Services;

public interface IUserService
{
    Task CreateAsync(UserRequestParams requestParams);
    Task UpdateAsync(UserRequestParams requestParams);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<Guid> CreateReturnGuidAsync(UserRequestParams requestParams);
    Task<UserDto> GetByGuidAsync(Guid guid);
    Task<UserDto> GetIdByGuidAsync(Guid guid);

}