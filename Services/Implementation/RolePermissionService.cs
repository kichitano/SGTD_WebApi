using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.RolePermission;

namespace SGTD_WebApi.Services.Implementation;

public class RolePermissionService : IRolePermissionService
{
    private readonly DatabaseContext _context;

    public RolePermissionService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(RolePermissionRequestParams requestParams)
    {
        var rolePermission = new RolePermission
        {
            RoleId = requestParams.RoleId,
            PermissionId = requestParams.PermissionId
        };
        _context.RolePermissions.Add(rolePermission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RolePermissionRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "RolePermission Id is required for update.");

        var rolePermission = await _context.RolePermissions.FirstOrDefaultAsync(rp => rp.Id == requestParams.Id);
        if (rolePermission == null)
            throw new KeyNotFoundException("RolePermission not found.");

        rolePermission.RoleId = requestParams.RoleId;
        rolePermission.PermissionId = requestParams.PermissionId;

        await _context.SaveChangesAsync();
    }

    public async Task<List<RolePermissionDto>> GetAllAsync()
    {
        return await _context.RolePermissions
            .Select(rp => new RolePermissionDto
            {
                Id = rp.Id,
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId
            })
            .ToListAsync();
    }

    public async Task<RolePermissionDto> GetByIdAsync(int id)
    {
        var rolePermission = await _context.RolePermissions.FirstOrDefaultAsync(rp => rp.Id == id);
        if (rolePermission == null)
            throw new KeyNotFoundException("RolePermission not found.");

        return new RolePermissionDto
        {
            Id = rolePermission.Id,
            RoleId = rolePermission.RoleId,
            PermissionId = rolePermission.PermissionId
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var rolePermission = await _context.RolePermissions.FirstOrDefaultAsync(rp => rp.Id == id);
        if (rolePermission == null)
            throw new KeyNotFoundException("RolePermission not found.");

        _context.RolePermissions.Remove(rolePermission);
        await _context.SaveChangesAsync();
    }
}