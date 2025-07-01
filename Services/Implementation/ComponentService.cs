using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Component;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar componentes o módulos del sistema.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar componentes,
/// con validación de nombres únicos y gestión de permisos asociados.
/// </summary>
public class ComponentService : IComponentService
{
    private readonly DatabaseContext _context;
    /// <summary>
    /// Inicializa una nueva instancia del servicio de componentes.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public ComponentService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un nuevo componente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del componente a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe un módulo con el mismo nombre.</exception>
    public async Task CreateAsync(ComponentRequestParams requestParams)
    {
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

    /// <summary>
    /// Actualiza un componente existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del componente.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID del componente es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el componente no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe otro módulo con el mismo nombre.</exception>
    public async Task UpdateAsync(ComponentRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Component Id is required for update.");
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == requestParams.Id);
        if (component == null)
            throw new KeyNotFoundException("Component not found.");

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

    /// <summary>
    /// Obtiene todos los componentes de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todos los componentes.</returns>
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

    /// <summary>
    /// Obtiene un componente específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del componente.</param>
    /// <returns>Un objeto DTO que representa el componente.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el componente no se encuentra.</exception>
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

    /// <summary>
    /// Elimina un componente por su identificador de forma asíncrona.
    /// También elimina lógicamente todos los permisos de rol asociados al componente.
    /// </summary>
    /// <param name="id">El identificador único del componente a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el componente no se encuentra.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == id);
        if (component == null)
            throw new KeyNotFoundException("Component not found.");

        var hasActiveRolePermissions = await _context.RoleComponentPermissions.AnyAsync(rcp => rcp.ComponentId == id && !rcp.IsDeleted);
        if (hasActiveRolePermissions)
        {
            var permissions = await _context.RoleComponentPermissions.Where(rcp => rcp.ComponentId == id && !rcp.IsDeleted).ToListAsync();
            foreach (var permission in permissions)
            {
                permission.IsDeleted = true;
                permission.DeletedAt = DateTime.UtcNow;
                permission.UpdatedAt = DateTime.UtcNow;
            }
        }

        component.IsDeleted = true;
        component.DeletedAt = DateTime.UtcNow;
        component.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}