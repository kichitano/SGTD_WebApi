using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.UserPosition;

namespace SGTD_WebApi.Services.Implementation;

public class UserPositionService : IUserPositionService
{
    private readonly DatabaseContext _context;

    public UserPositionService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(UserPositionRequestParams requestParams)
    {
        var userPosition = new UserPosition
        {
            UserId = requestParams.UserId,
            PositionId = requestParams.PositionId
        };
        _context.UserPositions.Add(userPosition);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserPositionRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "UserPosition Id is required for update.");

        var userPosition = await _context.UserPositions.FirstOrDefaultAsync(up => up.Id == requestParams.Id);
        if (userPosition == null)
            throw new KeyNotFoundException("UserPosition not found.");

        userPosition.UserId = requestParams.UserId;
        userPosition.PositionId = requestParams.PositionId;

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserPositionDto>> GetAllAsync()
    {
        return await _context.UserPositions
            .Select(up => new UserPositionDto
            {
                Id = up.Id,
                UserId = up.UserId,
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
            UserId = userPosition.UserId,
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