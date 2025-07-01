using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar roles del sistema.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar roles,
/// con validación de nombres únicos y control de dependencias con usuarios y permisos.
/// </summary>
public class RoleService : IRoleService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de roles.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public RoleService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un nuevo rol de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del rol a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe un rol con el mismo nombre.</exception>
    public async Task CreateAsync(RoleRequestParams requestParams)
    {
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

    /// <summary>
    /// Actualiza un rol existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del rol.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID del rol es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el rol no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe otro rol con el mismo nombre.</exception>
    public async Task UpdateAsync(RoleRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Role Id is required for update.");

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == requestParams.Id);
        if (role == null)
            throw new KeyNotFoundException("Role not found.");

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

    /// <summary>
    /// Obtiene todos los roles con su conteo de permisos de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todos los roles con información de permisos.</returns>
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

    /// <summary>
    /// Obtiene un rol específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del rol.</param>
    /// <returns>Un objeto DTO que representa el rol.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el rol no se encuentra.</exception>
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

    /// <summary>
    /// Elimina un rol por su identificador de forma asíncrona.
    /// Verifica que el rol no esté asignado a usuarios activos antes de eliminarlo.
    /// </summary>
    /// <param name="id">El identificador único del rol a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el rol no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el rol está asignado a usuarios activos.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        var hasActiveUsers = await _context.UserRoles.AnyAsync(ur => ur.RoleId == id && !ur.IsDeleted);
        if (hasActiveUsers)
        {
            throw new InvalidOperationException("No se puede eliminar el rol porque está asignado a usuarios activos.");
        }

        var hasActivePermissions = await _context.RoleComponentPermissions.AnyAsync(rcp => rcp.RoleId == id && !rcp.IsDeleted);
        if (hasActivePermissions)
        {
            var permissions = await _context.RoleComponentPermissions.Where(rcp => rcp.RoleId == id && !rcp.IsDeleted).ToListAsync();
            foreach (var permission in permissions)
            {
                permission.IsDeleted = true;
                permission.DeletedAt = DateTime.UtcNow;
                permission.UpdatedAt = DateTime.UtcNow;
            }
        }

        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;
        role.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Crea un nuevo rol y retorna su identificador de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del rol a crear.</param>
    /// <returns>El identificador del rol creado.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe un rol con el mismo nombre.</exception>
    public async Task<int> CreateReturnIdAsync(RoleRequestParams requestParams)
    {
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