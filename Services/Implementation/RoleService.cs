using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
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
        // Verificar si ya existe un rol con el mismo nombre
        var trimmedName = requestParams.Name?.Trim() ?? string.Empty;
        var existingRole = await _context.Roles
            .AnyAsync(r => r.Name.Trim().ToLower() == trimmedName.ToLower());
            
        if (existingRole)
        {
            throw new InvalidOperationException("Ya existe un rol con ese nombre.");
        }

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

        // Verificar si ya existe otro rol con el mismo nombre (excluyendo el actual)
        var trimmedName = requestParams.Name?.Trim() ?? string.Empty;
        var existingRole = await _context.Roles
            .AnyAsync(r => r.Id != requestParams.Id && r.Name.Trim().ToLower() == trimmedName.ToLower());
            
        if (existingRole)
        {
            throw new InvalidOperationException("Ya existe otro rol con ese nombre.");
        }

        role.Name = requestParams.Name;
        role.Description = requestParams.Description;
        role.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        return await _context.Roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                PermissionCount = _context.RoleComponentPermissions
                    .Count(rcp => rcp.RoleId == r.Id)
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

        // Verificar si el rol está siendo usado por usuarios activos
        var hasActiveUsers = await _context.UserRoles.AnyAsync(ur => ur.RoleId == id && !ur.IsDeleted);
        if (hasActiveUsers)
        {
            throw new InvalidOperationException("No se puede eliminar el rol porque está asignado a usuarios activos.");
        }

        // Verificar si el rol tiene permisos de componentes asociados activos
        var hasActivePermissions = await _context.RoleComponentPermissions.AnyAsync(rcp => rcp.RoleId == id && !rcp.IsDeleted);
        if (hasActivePermissions)
        {
            // Realizar eliminación lógica de permisos relacionados
            var permissions = await _context.RoleComponentPermissions.Where(rcp => rcp.RoleId == id && !rcp.IsDeleted).ToListAsync();
            foreach (var permission in permissions)
            {
                permission.IsDeleted = true;
                permission.DeletedAt = DateTime.UtcNow;
                permission.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Realizar eliminación lógica en lugar de física
        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;
        role.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<int> CreateReturnIdAsync(RoleRequestParams requestParams)
    {
        // Verificar si ya existe un rol con el mismo nombre
        var trimmedName = requestParams.Name?.Trim() ?? string.Empty;
        var existingRole = await _context.Roles
            .AnyAsync(r => r.Name.Trim().ToLower() == trimmedName.ToLower());
            
        if (existingRole)
        {
            throw new InvalidOperationException("Ya existe un rol con ese nombre.");
        }

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