using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.UserToken;

namespace SGTD_WebApi.Services;

public interface IUserTokenService
{
    public Task<string> GenerateTokenAsync(Guid userGuid);
    public Task InvalidateAllTokensAsync(UserTokenModel userTokenModel);
    public Task<bool> ValidateTokenAsync(string token);
    public Task<Guid?> GetUserGuidFromRefreshTokenAsync(string refreshToken);
    public Task<string> GetUserRefreshTokenFromGeneratedTokenAsync(string token);
}