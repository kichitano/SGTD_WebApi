using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.RoleComponentPermission;

namespace SGTD_WebApi.Services.Implementation;

public class RoleComponentPermissionService : IRoleComponentPermissionService
{
    private readonly DatabaseContext _context;
    public RoleComponentPermissionService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(RoleComponentPermissionRequestParams requestParams)
    {
        var roleComponentPermission = new RoleComponentPermission
        {
            RoleId = requestParams.RoleId,
            ComponentId = requestParams.ComponentId,
            PermissionId = requestParams.PermissionId
        };
        _context.RoleComponentPermissions.Add(roleComponentPermission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoleComponentPermissionRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "RoleComponentPermission Id is required for update.");

        var roleComponentPermission = await _context.RoleComponentPermissions
            .FirstOrDefaultAsync(rcp => rcp.Id == requestParams.Id);

        if (roleComponentPermission == null)
            throw new KeyNotFoundException("RoleComponentPermission not found.");

        roleComponentPermission.RoleId = requestParams.RoleId;
        roleComponentPermission.ComponentId = requestParams.ComponentId;
        roleComponentPermission.PermissionId = requestParams.PermissionId;
        await _context.SaveChangesAsync();
    }

    public async Task<List<RoleComponentPermissionDto>> GetAllAsync()
    {
        return await _context.RoleComponentPermissions
            .Select(rcp => new RoleComponentPermissionDto
            {
                Id = rcp.Id,
                RoleId = rcp.RoleId,
                ComponentId = rcp.ComponentId,
                PermissionId = rcp.PermissionId
            })
            .ToListAsync();
    }

    public async Task<RoleComponentPermissionDto> GetByIdAsync(int id)
    {
        var roleComponentPermission = await _context.RoleComponentPermissions
            .FirstOrDefaultAsync(rcp => rcp.Id == id);
        if (roleComponentPermission == null)
            throw new KeyNotFoundException("RoleComponentPermission not found.");

        return new RoleComponentPermissionDto
        {
            Id = roleComponentPermission.Id,
            RoleId = roleComponentPermission.RoleId,
            ComponentId = roleComponentPermission.ComponentId,
            PermissionId = roleComponentPermission.PermissionId
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var roleComponentPermission = await _context.RoleComponentPermissions
            .FirstOrDefaultAsync(rcp => rcp.Id == id);
        if (roleComponentPermission == null)
            throw new KeyNotFoundException("RoleComponentPermission not found.");
        _context.RoleComponentPermissions.Remove(roleComponentPermission);
        await _context.SaveChangesAsync();
    }

    public async Task CreateArrayAsync(RoleComponentPermissionRequestParams[] requestParams)
    {
        var roleId = requestParams.FirstOrDefault()?.RoleId;
        if (roleId.HasValue)
        {
            var existingPermissions = await _context.RoleComponentPermissions
                .Where(rcp => rcp.RoleId == roleId.Value)
                .ToListAsync();

            if (existingPermissions.Any())
            {
                _context.RoleComponentPermissions.RemoveRange(existingPermissions);
                await _context.SaveChangesAsync();
            }
        }

        var distinctPermissions = requestParams
            .GroupBy(rcp => new { rcp.RoleId, rcp.ComponentId, rcp.PermissionId })
            .Select(group => group.First())
            .ToList();

        var roleComponentPermissions = distinctPermissions.Select(rcp => new RoleComponentPermission
        {
            RoleId = rcp.RoleId,
            ComponentId = rcp.ComponentId,
            PermissionId = rcp.PermissionId
        }).ToList();

        _context.RoleComponentPermissions.AddRange(roleComponentPermissions);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateArrayAsync(int roleId, RoleComponentPermissionRequestParams[] requestParams)
    {
        var existingPermissions = await _context.RoleComponentPermissions
            .Where(rcp => rcp.RoleId == roleId)
            .ToListAsync();

        if (existingPermissions.Any())
        {
            _context.RoleComponentPermissions.RemoveRange(existingPermissions);
            await _context.SaveChangesAsync();
        }

        var roleComponentPermissions = requestParams.Select(rcp => new RoleComponentPermission
        {
            RoleId = roleId,
            ComponentId = rcp.ComponentId,
            PermissionId = rcp.PermissionId
        }).ToList();

        _context.RoleComponentPermissions.AddRange(roleComponentPermissions);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RoleComponentPermissionDto>> GetByRoleIdAsync(int roleId)
    {
        return await _context.RoleComponentPermissions
            .Where(rcp => rcp.RoleId == roleId)
            .Select(rcp => new RoleComponentPermissionDto
            {
                Id = rcp.Id,
                RoleId = rcp.RoleId,
                ComponentId = rcp.ComponentId,
                PermissionId = rcp.PermissionId
            })
            .ToListAsync();
    }
}