using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services.Implementation;

public class RoleService : IRoleService
{
    private readonly DatabaseContext _context;

    public RoleService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(RoleRequestParams requestParams)
    {
        var role = new Role
        {
            Name = requestParams.Name,
            Description = requestParams.Description
        };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoleRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Role Id is required for update.");

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == requestParams.Id);
        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        role.Name = requestParams.Name;
        role.Description = requestParams.Description;

        await _context.SaveChangesAsync();
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        return await _context.Roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            })
            .ToListAsync();
    }

    public async Task<RoleDto> GetByIdAsync(int id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CreateReturnIdAsync(RoleRequestParams requestParams)
    {
        var role = new Role
        {
            Name = requestParams.Name,
            Description = requestParams.Description
        };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return role.Id;
    }
}