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
            PositionId = requestParams.PositionId
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
        user.PositionId = requestParams.PositionId;

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Select(q => new UserDto
            {
                UserGuid = q.UserGuid,
                PersonId = q.PersonId,
                Email = q.Email,
                StorageSize = q.StorageSize,
                Status = q.Status,
                Person = q.Person,
                PositionId = q.Position!.Id,
                PositionName = q.Position.Name
            })
            .ToListAsync();

        return users;
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Select(q => new UserDto
            {
                Id = q.Id,
                UserGuid = q.UserGuid,
                PersonId = q.PersonId,
                Email = q.Email,
                StorageSize = q.StorageSize,
                Status = q.Status,
                Person = q.Person,
                PositionId = q.Position!.Id,
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
                PositionId = requestParams.PositionId
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
            .Select(q => new UserDto
            {
                UserGuid = q.UserGuid,
                PersonId = q.PersonId,
                Email = q.Email,
                StorageSize = q.StorageSize,
                Status = q.Status,
                Person = q.Person,
                PositionId = q.PositionId ?? 0
            })
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
                PositionId = q.PositionId ?? 0
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