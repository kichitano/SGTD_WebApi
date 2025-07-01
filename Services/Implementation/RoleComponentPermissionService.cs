using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.RoleComponentPermission;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar los permisos de componentes asignados a roles.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar
/// las relaciones entre roles, componentes y permisos del sistema.
/// </summary>
public class RoleComponentPermissionService : IRoleComponentPermissionService
{
    private readonly DatabaseContext _context;
    /// <summary>
    /// Inicializa una nueva instancia del servicio de permisos de componentes de rol.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public RoleComponentPermissionService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea una nueva relación de permiso de componente para un rol de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del permiso a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
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

    /// <summary>
    /// Actualiza una relación existente de permiso de componente para un rol de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del permiso.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID del permiso es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el permiso no se encuentra.</exception>
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

    /// <summary>
    /// Obtiene todas las relaciones de permisos de componentes de roles de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todas las relaciones de permisos.</returns>
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

    /// <summary>
    /// Obtiene una relación específica de permiso de componente por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la relación de permiso.</param>
    /// <returns>Un objeto DTO que representa la relación de permiso.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la relación de permiso no se encuentra.</exception>
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

    /// <summary>
    /// Elimina una relación de permiso de componente por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la relación de permiso a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la relación de permiso no se encuentra.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var roleComponentPermission = await _context.RoleComponentPermissions
            .FirstOrDefaultAsync(rcp => rcp.Id == id);
        if (roleComponentPermission == null)
            throw new KeyNotFoundException("RoleComponentPermission not found.");
        _context.RoleComponentPermissions.Remove(roleComponentPermission);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Crea múltiples relaciones de permisos de componentes para un rol de forma asíncrona.
    /// Elimina los permisos existentes del rol antes de crear los nuevos.
    /// </summary>
    /// <param name="requestParams">Array de parámetros que contienen la información de los permisos a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
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

    /// <summary>
    /// Actualiza todas las relaciones de permisos de componentes para un rol específico de forma asíncrona.
    /// Elimina los permisos existentes del rol y crea los nuevos permisos especificados.
    /// </summary>
    /// <param name="roleId">El identificador del rol cuyos permisos se van a actualizar.</param>
    /// <param name="requestParams">Array de parámetros que contienen la información de los nuevos permisos.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
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

    /// <summary>
    /// Obtiene todas las relaciones de permisos de componentes para un rol específico de forma asíncrona.
    /// </summary>
    /// <param name="roleId">El identificador del rol.</param>
    /// <returns>Una lista de objetos DTO que representan los permisos del rol especificado.</returns>
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