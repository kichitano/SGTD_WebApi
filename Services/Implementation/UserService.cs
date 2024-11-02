using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.User;

namespace SGTD_WebApi.Services.Implementation;

public class UserService : IUserService
{
    private readonly DatabaseContext _context;

    public UserService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(UserRequestParams requestParams)
    {
        var user = new User
        {
            PersonId = requestParams.PersonId,
            Email = requestParams.Email,
            Password = HashPassword(requestParams.Password),
            StorageSize = requestParams.StorageSize,
            Status = requestParams.Status,
            CreatedAt = DateTime.Now,
            UserGuid = Guid.NewGuid(),
            FolderPath = GenerateFolderPath()
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
            user.Password = HashPassword(requestParams.Password);
        }

        user.StorageSize = requestParams.StorageSize;
        user.Status = requestParams.Status;

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
                Position = q.userPosition.Position
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
                    Position = userPosition.Position
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
                Password = HashPassword(requestParams.Password),
                StorageSize = requestParams.StorageSize,
                Status = requestParams.Status,
                CreatedAt = DateTime.Now,
                UserGuid = Guid.NewGuid(),
                FolderPath = GenerateFolderPath()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.UserGuid;
        }

        throw new InvalidOperationException("Position name already exists.");
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
                    Position = userPosition!.Position
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
                Person = q.Person
            })
            .FirstOrDefaultAsync();

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return user;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    private string GenerateFolderPath()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 12);
    }
}