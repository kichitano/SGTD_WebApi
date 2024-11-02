using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.UserPosition;

namespace SGTD_WebApi.Services.Implementation;

public class UserPositionService : IUserPositionService
{
    private readonly DatabaseContext _context;
    private readonly IUserService _userService;

    public UserPositionService(DatabaseContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task CreateAsync(UserPositionRequestParams requestParams)
    {
        var user = await _userService.GetIdByGuidAsync(requestParams.UserGuid);

        var userPosition = new UserPosition
        {
            UserId = user.Id ?? 0,
            UserGuid = requestParams.UserGuid,
            PositionId = requestParams.PositionId
        };
        _context.UserPositions.Add(userPosition);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserPositionRequestParams requestParams)
    {
        var userPosition = await _context.UserPositions.FirstOrDefaultAsync(up => up.UserGuid == requestParams.UserGuid);
        if (userPosition == null)
            throw new KeyNotFoundException("UserPosition not found.");

        userPosition.PositionId = requestParams.PositionId;

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserPositionDto>> GetAllAsync()
    {
        return await _context.UserPositions
            .Select(up => new UserPositionDto
            {
                Id = up.Id,
                UserGuid = up.UserGuid,
                PositionId = up.PositionId
            })
            .ToListAsync();
    }

    public async Task<UserPositionDto> GetByIdAsync(int id)
    {
        var userPosition = await _context.UserPositions.FirstOrDefaultAsync(up => up.Id == id);
        if (userPosition == null)
            throw new KeyNotFoundException("UserPosition not found.");

        return new UserPositionDto
        {
            Id = userPosition.Id,
            UserGuid = userPosition.UserGuid,
            PositionId = userPosition.PositionId
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var userPosition = await _context.UserPositions.FirstOrDefaultAsync(up => up.Id == id);
        if (userPosition == null)
            throw new KeyNotFoundException("UserPosition not found.");

        _context.UserPositions.Remove(userPosition);
        await _context.SaveChangesAsync();
    }
}