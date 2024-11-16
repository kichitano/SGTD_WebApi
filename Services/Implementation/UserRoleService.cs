using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
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
            throw new ArgumentNullException(nameof(requestParams.Id), "UserRoles Id is required for update.");

        var userRole = await _context.UserRoles.FirstOrDefaultAsync(pr => pr.Id == requestParams.Id);
        if (userRole == null)
            throw new KeyNotFoundException("UserRole not found.");

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
            throw new KeyNotFoundException("UserRole not found.");

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
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(pr => pr.Id == user.Id);
        if (userRole == null)
            throw new KeyNotFoundException("UserRole not found.");

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserRoleDto>> GetByUserGuidAsync(Guid userGuid)
    {
        var user = await _userService.GetIdByGuidAsync(userGuid);
        return await _context.UserRoles
            .Where(pr => pr.UserId == user.Id)
            .Select(pr => new UserRoleDto
            {
                Id = pr.Id,
                UserId = pr.UserId,
                RoleId = pr.RoleId,
            })
            .ToListAsync();
    }
}