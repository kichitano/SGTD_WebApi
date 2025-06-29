using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Component;

namespace SGTD_WebApi.Services.Implementation;

public class ComponentService : IComponentService
{
    private readonly DatabaseContext _context;
    public ComponentService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(ComponentRequestParams requestParams)
    {
        // Verificar si ya existe un componente con el mismo nombre
        var trimmedName = requestParams.Name?.Trim() ?? string.Empty;
        var existingComponent = await _context.Components
            .AnyAsync(c => c.Name.Trim().ToLower() == trimmedName.ToLower());
            
        if (existingComponent)
        {
            throw new InvalidOperationException("Ya existe un módulo con ese nombre.");
        }

        var component = new Component
        {
            Name = requestParams.Name
        };
        _context.Components.Add(component);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ComponentRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Component Id is required for update.");
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == requestParams.Id);
        if (component == null)
            throw new KeyNotFoundException("Component not found.");

        // Verificar si ya existe otro componente con el mismo nombre (excluyendo el actual)
        var trimmedName = requestParams.Name?.Trim() ?? string.Empty;
        var existingComponent = await _context.Components
            .AnyAsync(c => c.Id != requestParams.Id && c.Name.Trim().ToLower() == trimmedName.ToLower());
            
        if (existingComponent)
        {
            throw new InvalidOperationException("Ya existe otro módulo con ese nombre.");
        }

        component.Name = requestParams.Name;
        component.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<ComponentDto>> GetAllAsync()
    {
        return await _context.Components
            .Select(c => new ComponentDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<ComponentDto> GetByIdAsync(int id)
    {
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == id);
        if (component == null)
            throw new KeyNotFoundException("Component not found.");
        return new ComponentDto
        {
            Id = component.Id,
            Name = component.Name
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == id);
        if (component == null)
            throw new KeyNotFoundException("Component not found.");

        // Verificar si el componente tiene permisos de roles asociados activos
        var hasActiveRolePermissions = await _context.RoleComponentPermissions.AnyAsync(rcp => rcp.ComponentId == id && !rcp.IsDeleted);
        if (hasActiveRolePermissions)
        {
            // Realizar eliminación lógica de permisos relacionados
            var permissions = await _context.RoleComponentPermissions.Where(rcp => rcp.ComponentId == id && !rcp.IsDeleted).ToListAsync();
            foreach (var permission in permissions)
            {
                permission.IsDeleted = true;
                permission.DeletedAt = DateTime.UtcNow;
                permission.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Realizar eliminación lógica en lugar de física
        component.IsDeleted = true;
        component.DeletedAt = DateTime.UtcNow;
        component.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}