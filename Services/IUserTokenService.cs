using SGTD_WebApi.Models.UserToken;

namespace SGTD_WebApi.Services;

public interface IUserTokenService
{
    Task<string> GenerateTokenAsync(Guid userGuid);
    Task InvalidateAllTokensAsync(UserTokenModel userTokenModel);
    Task<bool> ValidateTokenAsync(string token);
    Task<Guid?> GetUserGuidFromRefreshTokenAsync(string refreshToken);
    Task<string> GetUserRefreshTokenFromGeneratedTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string refreshToken);
}