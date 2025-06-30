using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.PositionRole;
using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services.Implementation;

public class UserRoleService : IUserRoleService
{
    private readonly DatabaseContext _context;
    private readonly IUserService _userService;

    public UserRoleService(DatabaseContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

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

        // Eliminación lógica de todos los roles del usuario
        foreach (var userRole in userRoles)
        {
            userRole.IsDeleted = true;
            userRole.DeletedAt = DateTime.UtcNow;
            userRole.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

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