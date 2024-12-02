using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Helpers;
using SGTD_WebApi.Models.User;

namespace SGTD_WebApi.Services.Implementation;

public class UserService : IUserService
{
    private readonly DatabaseContext _context;
    private readonly EncryptHelper _encryptHelper;

    public UserService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _encryptHelper = new EncryptHelper(configuration);
    }

    public async Task CreateAsync(UserRequestParams requestParams)
    {
        var user = new User
        {
            PersonId = requestParams.PersonId,
            Email = requestParams.Email,
            Password = _encryptHelper.PasswordEncrypt(requestParams.Password),
            StorageSize = requestParams.StorageSize,
            Status = requestParams.Status,
            CreatedAt = DateTime.Now,
            UserGuid = Guid.NewGuid(),
            FolderPath = GenerateFolderPath(),
            AreaId = requestParams.AreaId
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserRequestParams requestParams)
    {
        if (requestParams.UserGuid == null)
            throw new ArgumentNullException(nameof(requestParams.UserGuid), "User Id is required for update.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserGuid == requestParams.UserGuid);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.PersonId = requestParams.PersonId;
        user.Email = requestParams.Email;

        if (!string.IsNullOrEmpty(requestParams.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(requestParams.Password, workFactor: 12);
        }

        user.StorageSize = requestParams.StorageSize;
        user.Status = requestParams.Status;
        user.AreaId = requestParams.AreaId;

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Join(_context.UserPositions,
                u => u.Id,
                up => up.User.Id,
                (u, up) => new { user = u, userPosition = up})
            .Select(q => new UserDto
            {
                UserGuid = q.user.UserGuid,
                PersonId = q.user.PersonId,
                Email = q.user.Email,
                StorageSize = q.user.StorageSize,
                Status = q.user.Status,
                Person = q.user.Person,
                Position = q.userPosition.Position,
                AreaId = q.user.AreaId ?? 0
            })
            .ToListAsync();

        return users;
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Join(
                _context.UserPositions,
                user => user.Id,
                userPosition => userPosition.Id,
                (user, userPosition) => new UserDto
                {
                    Id = user.Id,
                    UserGuid = user.UserGuid,
                    PersonId = user.PersonId,
                    Email = user.Email,
                    StorageSize = user.StorageSize,
                    Status = user.Status,
                    Person = user.Person,
                    Position = userPosition.Position,
                    AreaId = user.AreaId ?? 0
                }).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return user;
    }

    public async Task DeleteByIdAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> CreateReturnGuidAsync(UserRequestParams requestParams)
    {
        if (!string.IsNullOrWhiteSpace(requestParams.Email))
        {
            var user = new User
            {
                PersonId = requestParams.PersonId,
                Email = requestParams.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(requestParams.Password, workFactor: 12),
                StorageSize = requestParams.StorageSize,
                Status = requestParams.Status,
                CreatedAt = DateTime.Now,
                UserGuid = Guid.NewGuid(),
                FolderPath = GenerateFolderPath(),
                AreaId = requestParams.AreaId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.UserGuid;
        }

        throw new InvalidOperationException("User email already exists.");
    }

    public async Task<UserDto> GetByGuidAsync(Guid guid)
    {
        var user = await _context.Users
            .Where(u => u.UserGuid.Equals(guid))
            .GroupJoin(
                _context.UserPositions,
                u => u.Id,
                up => up.UserId,
                (u, userPositions) => new { User = u, UserPositions = userPositions.DefaultIfEmpty() }
            )
            .SelectMany(
                result => result.UserPositions,
                (result, userPosition) => new UserDto
                {
                    UserGuid = result.User.UserGuid,
                    PersonId = result.User.PersonId,
                    Email = result.User.Email,
                    StorageSize = result.User.StorageSize,
                    Status = result.User.Status,
                    Person = result.User.Person,
                    Position = userPosition!.Position,
                    AreaId = result.User.AreaId ?? 0
                }
            )
            .FirstOrDefaultAsync();

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return user;
    }

    public async Task<UserDto> GetIdByGuidAsync(Guid guid)
    {
        var user = await _context.Users
            .Where(q => q.UserGuid.Equals(guid))
            .Select(q => new UserDto
            {
                Id = q.Id,
                UserGuid = q.UserGuid,
                PersonId = q.PersonId,
                Email = q.Email,
                StorageSize = q.StorageSize,
                Status = q.Status,
                Person = q.Person,
                AreaId = q.AreaId ?? 0
            })
            .FirstOrDefaultAsync();

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return user;
    }

    private string GenerateFolderPath()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 12);
    }
}