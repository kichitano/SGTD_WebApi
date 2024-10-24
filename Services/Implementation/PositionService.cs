using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.Position;

namespace SGTD_WebApi.Services.Implementation;

public class PositionService : IPositionService
{
    private readonly DatabaseContext _context;

    public PositionService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(PositionRequestParams requestParams)
    {
        var position = new Position
        {
            Name = requestParams.Name,
            Description = requestParams.Description,
            AreaId = requestParams.AreaId,
            Hierarchy = requestParams.Hierarchy
        };
        _context.Positions.Add(position);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PositionRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Position Id is required for update.");

        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == requestParams.Id);
        if (position == null)
            throw new KeyNotFoundException("Position not found.");

        position.Name = requestParams.Name;
        position.Description = requestParams.Description;
        position.AreaId = requestParams.AreaId;
        position.Hierarchy = requestParams.Hierarchy;

        await _context.SaveChangesAsync();
    }

    public async Task<List<PositionDto>> GetAllAsync()
    {
        return await _context.Positions
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                AreaId = p.AreaId,
                Hierarchy = p.Hierarchy
            })
            .ToListAsync();
    }

    public async Task<PositionDto> GetByIdAsync(int id)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
        if (position == null)
            throw new KeyNotFoundException("Position not found.");

        return new PositionDto
        {
            Id = position.Id,
            Name = position.Name,
            Description = position.Description,
            AreaId = position.AreaId,
            Hierarchy = position.Hierarchy
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
        if (position == null)
            throw new KeyNotFoundException("Position not found.");

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();
    }
}