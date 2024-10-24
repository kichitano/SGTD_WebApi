using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.Permission;

namespace SGTD_WebApi.Services.Implementation;

public class PermissionService : IPermissionService
{
    private readonly DatabaseContext _context;

    public PermissionService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(PermissionRequestParams requestParams)
    {
        var permission = new Permission
        {
            Name = requestParams.Name,
            Description = requestParams.Description
        };
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PermissionRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Permission Id is required for update.");

        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == requestParams.Id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        permission.Name = requestParams.Name;
        permission.Description = requestParams.Description;

        await _context.SaveChangesAsync();
    }

    public async Task<List<PermissionDto>> GetAllAsync()
    {
        return await _context.Permissions
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync();
    }

    public async Task<PermissionDto> GetByIdAsync(int id)
    {
        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();
    }
}