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
            Status = requestParams.Status,
            CreatedAt = DateTime.Now,
            UserGuid = Guid.NewGuid()
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "User Id is required for update.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == requestParams.Id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.PersonId = requestParams.PersonId;
        user.Email = requestParams.Email;
        if (!string.IsNullOrEmpty(requestParams.Password))
        {
            user.Password = HashPassword(requestParams.Password);
        }
        user.Status = requestParams.Status;

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        return await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                PersonId = u.PersonId,
                Email = u.Email,
                Status = u.Status
            })
            .ToListAsync();
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return new UserDto
        {
            Id = user.Id,
            PersonId = user.PersonId,
            Email = user.Email,
            Status = user.Status
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}