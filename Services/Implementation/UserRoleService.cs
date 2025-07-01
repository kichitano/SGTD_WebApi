using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.PositionRole;
using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar las asignaciones de roles a usuarios.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar
/// las relaciones entre usuarios y roles del sistema.
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly DatabaseContext _context;
    private readonly IUserService _userService;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de roles de usuarios.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    /// <param name="userService">Servicio de usuarios para operaciones relacionadas.</param>
    public UserRoleService(DatabaseContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    /// <summary>
    /// Crea una nueva asignación de rol a un usuario de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información de la asignación de rol.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task CreateAsync(UserRoleRequestParams requestParams)
    {
        var user = await _userService.GetIdByGuidAsync(requestParams.UserGuid);
        var positionRole = new UserRole
        {
            UserId = user.Id ?? 0,
            RoleId = requestParams.RoleId
        };
        _context.UserRoles.Add(positionRole);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza una asignación existente de rol a usuario de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada de la asignación de rol.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID de la asignación es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la asignación de rol no se encuentra.</exception>
    public async Task UpdateAsync(UserRoleRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "ID de rol de usuario requerido para actualización.");

        var userRole = await _context.UserRoles.FirstOrDefaultAsync(pr => pr.Id == requestParams.Id);
        if (userRole == null)
            throw new KeyNotFoundException("Rol de usuario no encontrado.");

        var user = await _userService.GetIdByGuidAsync(requestParams.UserGuid);
        userRole.UserId = user.Id ?? 0;
        userRole.RoleId = requestParams.RoleId;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene todas las asignaciones de roles a usuarios de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todas las asignaciones de roles.</returns>
    public async Task<List<UserRoleDto>> GetAllAsync()
    {
        return await _context.UserRoles
            .Select(pr => new UserRoleDto
            {
                Id = pr.Id,
                UserId = pr.UserId,
                RoleId = pr.RoleId
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una asignación específica de rol por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la asignación de rol.</param>
    /// <returns>Un objeto DTO que representa la asignación de rol.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la asignación de rol no se encuentra.</exception>
    public async Task<UserRoleDto> GetByIdAsync(int id)
    {
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(pr => pr.Id == id);
        if (userRole == null)
            throw new KeyNotFoundException("Rol de usuario no encontrado.");

        return new UserRoleDto
        {
            Id = userRole.Id,
            UserId = userRole.UserId,
            RoleId = userRole.RoleId
        };
    }

    /// <summary>
    /// Elimina todas las asignaciones de roles de un usuario por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">El identificador único del usuario.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el usuario no se encuentra.</exception>
    public async Task DeleteByUserGuidAsync(Guid userGuid)
    {
        var user = await _userService.GetIdByGuidAsync(userGuid);
        if (user?.Id == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        var userRoles = await _context.UserRoles
            .Where(pr => pr.UserId == user.Id && !pr.IsDeleted)
            .ToListAsync();

        if (!userRoles.Any())
            return; // No hay roles que eliminar, no es un error

        foreach (var userRole in userRoles)
        {
            userRole.IsDeleted = true;
            userRole.DeletedAt = DateTime.UtcNow;
            userRole.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene todas las asignaciones de roles de un usuario específico de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">El identificador único del usuario.</param>
    /// <returns>Una lista de objetos DTO que representan las asignaciones de roles del usuario.</returns>
    public async Task<List<UserRoleDto>> GetByUserGuidAsync(Guid userGuid)
    {
        var user = await _userService.GetIdByGuidAsync(userGuid);
        return await _context.UserRoles
            .Where(pr => pr.UserId == user.Id && !pr.IsDeleted)
            .Select(pr => new UserRoleDto
            {
                Id = pr.Id,
                UserId = pr.UserId,
                RoleId = pr.RoleId,
            })
            .ToListAsync();
    }
}