using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.PositionRole;
using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services.Implementation;

public class PositionRoleService : IPositionRoleService
{
    private readonly DatabaseContext _context;

    public PositionRoleService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(PositionRoleRequestParams requestParams)
    {
        var positionRole = new PositionRole
        {
            PositionId = requestParams.PositionId,
            RoleId = requestParams.RoleId
        };
        _context.PositionRoles.Add(positionRole);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PositionRoleRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "PositionRole Id is required for update.");

        var positionRole = await _context.PositionRoles.FirstOrDefaultAsync(pr => pr.Id == requestParams.Id);
        if (positionRole == null)
            throw new KeyNotFoundException("PositionRole not found.");

        positionRole.PositionId = requestParams.PositionId;
        positionRole.RoleId = requestParams.RoleId;

        await _context.SaveChangesAsync();
    }

    public async Task<List<PositionRoleDto>> GetAllAsync()
    {
        return await _context.PositionRoles
            .Select(pr => new PositionRoleDto
            {
                Id = pr.Id,
                PositionId = pr.PositionId,
                RoleId = pr.RoleId
            })
            .ToListAsync();
    }

    public async Task<PositionRoleDto> GetByIdAsync(int id)
    {
        var positionRole = await _context.PositionRoles.FirstOrDefaultAsync(pr => pr.Id == id);
        if (positionRole == null)
            throw new KeyNotFoundException("PositionRole not found.");

        return new PositionRoleDto
        {
            Id = positionRole.Id,
            PositionId = positionRole.PositionId,
            RoleId = positionRole.RoleId
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var positionRole = await _context.PositionRoles.FirstOrDefaultAsync(pr => pr.Id == id);
        if (positionRole == null)
            throw new KeyNotFoundException("PositionRole not found.");

        _context.PositionRoles.Remove(positionRole);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PositionRoleDto>> GetByPositionIdAsync(int positionId)
    {
        return await _context.PositionRoles
            .Where(pr => pr.PositionId == positionId)
            .Select(pr => new PositionRoleDto
            {
                Id = pr.Id,
                PositionId = pr.PositionId,
                RoleId = pr.RoleId,
            })
            .ToListAsync();
    }
}