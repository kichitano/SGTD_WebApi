using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Permission;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar permisos del sistema.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar permisos,
/// con validación de nombres únicos.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de permisos.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public PermissionService(
        DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un nuevo permiso de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del permiso a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el nombre del permiso ya existe.</exception>
    public async Task CreateAsync(PermissionRequestParams requestParams)
    {
        if (await IsPermissionNameUniqueAsync(requestParams.Name))
        {
            var permission = new Permission
            {
                Name = requestParams.Name
            };
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("Permission name already exists.");
        }
    }

    /// <summary>
    /// Actualiza un permiso existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del permiso.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID del permiso es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el permiso no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el nombre del permiso ya existe.</exception>
    public async Task UpdateAsync(PermissionRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Permission Id is required for update.");

        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == requestParams.Id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        if (await IsPermissionNameUniqueAsync(requestParams.Name, requestParams.Id))
        {
            permission.Name = requestParams.Name;
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("Permission name already exists.");
        }
    }

    /// <summary>
    /// Obtiene todos los permisos de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todos los permisos.</returns>
    public async Task<List<PermissionDto>> GetAllAsync()
    {
        return await _context.Permissions
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un permiso específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del permiso.</param>
    /// <returns>Un objeto DTO que representa el permiso.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el permiso no se encuentra.</exception>
    public async Task<PermissionDto> GetByIdAsync(int id)
    {
        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name
        };
    }

    /// <summary>
    /// Elimina un permiso por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del permiso a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el permiso no se encuentra.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Verifica si el nombre de un permiso es único de forma asíncrona.
    /// </summary>
    /// <param name="name">El nombre del permiso a verificar.</param>
    /// <param name="excludePermissionId">ID del permiso a excluir de la verificación (opcional).</param>
    /// <returns>True si el nombre es único, false en caso contrario.</returns>
    private async Task<bool> IsPermissionNameUniqueAsync(string name, int? excludePermissionId = null)
    {
        var query = _context.Permissions
            .Where(p => p.Name == name);

        if (excludePermissionId.HasValue)
            query = query.Where(p => p.Id != excludePermissionId.Value);

        return !await query.AnyAsync();
    }
}