using SGTD_WebApi.Models.User;

namespace SGTD_WebApi.Services;

public interface IUserService : IBaseService<UserRequestParams, UserDto>;