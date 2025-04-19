using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Helpers;
using SGTD_WebApi.Models.UserToken;

namespace SGTD_WebApi.Services.Implementation;

public class UserTokenService : IUserTokenService
{
    private readonly DatabaseContext _context;
    private readonly JwtHelper _jwtHelper;

    public UserTokenService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _jwtHelper = new JwtHelper(configuration);
    }

    public async Task<string> GenerateTokenAsync(Guid userGuid)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(q => q.UserGuid.Equals(userGuid));

        if (user == null)
        {
            return string.Empty;
        }

        var token = _jwtHelper.GenerateJwtToken(user);
        var refreshToken = Guid.NewGuid().ToString();

        var userToken = new UserToken
        {
            UserGuid = user.UserGuid,
            Token = token,
            RefreshToken = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(3),
            IsActive = true
        };

        _context.UserTokens.Add(userToken);
        await _context.SaveChangesAsync();

        await InvalidateAllTokensAsync(new UserTokenModel
        { UserGuid = userToken.UserGuid, Token = userToken.Token });

        return token;
    }

    public async Task<Guid?> GetUserGuidFromRefreshTokenAsync(string refreshToken)
    {
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken && t.IsActive && t.ExpiresAt > DateTime.UtcNow);
        return userToken?.UserGuid;
    }

    public async Task<string> GetUserRefreshTokenFromGeneratedTokenAsync(string token)
    {
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.IsActive && t.ExpiresAt > DateTime.UtcNow);
        return userToken?.RefreshToken ?? string.Empty;
    }

    public async Task InvalidateAllTokensAsync(UserTokenModel userTokenModel)
    {
        var userTokens = await _context.UserTokens
            .Where(q => q.UserGuid.Equals(userTokenModel.UserGuid) && q.Token != userTokenModel.Token)
            .ToListAsync();

        if (userTokens.Count > 0)
        {
            foreach (var userToken in userTokens)
            {
                userToken.IsActive = false;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var response = false;
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t =>
                t.Token == token);

        if (userToken == null)
        {
            return false;
        }

        if (userToken.IsActive && userToken.ExpiresAt > DateTime.UtcNow)
        {
            response = true;
        }
        else
        {
            await InvalidateAllTokensAsync(new UserTokenModel
                { UserGuid = userToken.UserGuid, Token = userToken.Token });
        }
        return response;
    }
}