using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Helpers;
using SGTD_WebApi.Models.User;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar usuarios del sistema.
/// Proporciona funcionalidades completas para crear, actualizar, consultar y eliminar usuarios,
/// incluyendo encriptación de contraseñas y gestión de tokens y roles asociados.
/// </summary>
public class UserService : IUserService
{
    private readonly DatabaseContext _context;
    private readonly EncryptHelper _encryptHelper;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de usuarios.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    /// <param name="configuration">Configuración de la aplicación para configuraciones de encriptación.</param>
    public UserService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _encryptHelper = new EncryptHelper(configuration);
    }

    /// <summary>
    /// Crea un nuevo usuario de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del usuario a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe un usuario con el mismo email o persona asignada.</exception>
    public async Task CreateAsync(UserRequestParams requestParams)
    {
        var existingUserByEmail = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == requestParams.Email.ToLower());
            
        if (existingUserByEmail)
        {
            throw new InvalidOperationException("Ya existe un usuario con ese email.");
        }

        var existingUserByPerson = await _context.Users
            .AnyAsync(u => u.PersonId == requestParams.PersonId);
            
        if (existingUserByPerson)
        {
            throw new InvalidOperationException("Ya existe un usuario asignado a esa persona.");
        }

        var user = new User
        {
            PersonId = requestParams.PersonId,
            Email = requestParams.Email,
            Password = _encryptHelper.PasswordEncrypt(requestParams.Password),
            StorageSize = requestParams.StorageSize,
            Status = requestParams.Status,
            CreatedAt = DateTime.UtcNow,
            UserGuid = Guid.NewGuid(),
            FolderPath = GenerateFolderPath(),
            PositionId = requestParams.PositionId
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza un usuario existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del usuario.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el GUID del usuario es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el usuario no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe otro usuario con el mismo email o persona asignada.</exception>
    public async Task UpdateAsync(UserRequestParams requestParams)
    {
        if (requestParams.UserGuid == null)
            throw new ArgumentNullException(nameof(requestParams.UserGuid), "ID de usuario requerido para actualización.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserGuid == requestParams.UserGuid);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        var existingUserByEmail = await _context.Users
            .AnyAsync(u => u.UserGuid != requestParams.UserGuid && u.Email.ToLower() == requestParams.Email.ToLower());
            
        if (existingUserByEmail)
        {
            throw new InvalidOperationException("Ya existe otro usuario con ese email.");
        }

        var existingUserByPerson = await _context.Users
            .AnyAsync(u => u.UserGuid != requestParams.UserGuid && u.PersonId == requestParams.PersonId);
            
        if (existingUserByPerson)
        {
            throw new InvalidOperationException("Ya existe otro usuario asignado a esa persona.");
        }

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

    /// <summary>
    /// Obtiene todos los usuarios activos de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todos los usuarios activos.</returns>
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
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
                PositionName = q.Position.Name
            })
            .Where(q => q.Status)
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
            throw new KeyNotFoundException("Usuario no encontrado.");

        return user;
    }

    public async Task DeleteByGuidAsync(UserDeletedRequestParams requestParams)
    {
        if (requestParams.UserGuid == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        var user = await _context.Users
            .Where(u => u.UserGuid.Equals(requestParams.UserGuid))
            .FirstOrDefaultAsync();

        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        var tokens = await _context.UserTokens.Where(t => t.UserGuid == user.UserGuid).ToListAsync();
        foreach (var token in tokens)
        {
            token.IsDeleted = true;
            token.DeletedAt = DateTime.UtcNow;
            token.UpdatedAt = DateTime.UtcNow;
        }

        var roles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
        foreach (var role in roles)
        {
            role.IsDeleted = true;
            role.DeletedAt = DateTime.UtcNow;
            role.UpdatedAt = DateTime.UtcNow;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error al eliminar el usuario: " + ex.Message, ex);
        }
    }

    public async Task<Guid> CreateReturnGuidAsync(UserRequestParams requestParams)
    {
        if (!string.IsNullOrWhiteSpace(requestParams.Email))
        {
                var existingUserByEmail = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == requestParams.Email.ToLower());
                
            if (existingUserByEmail)
            {
                throw new InvalidOperationException("Ya existe un usuario con ese email.");
            }

                var existingUserByPerson = await _context.Users
                .AnyAsync(u => u.PersonId == requestParams.PersonId);
                
            if (existingUserByPerson)
            {
                throw new InvalidOperationException("Ya existe un usuario asignado a esa persona.");
            }

            var user = new User
            {
                PersonId = requestParams.PersonId,
                Email = requestParams.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(requestParams.Password, workFactor: 12),
                StorageSize = requestParams.StorageSize,
                Status = requestParams.Status,
                CreatedAt = DateTime.UtcNow,
                UserGuid = Guid.NewGuid(),
                FolderPath = GenerateFolderPath(),
                PositionId = requestParams.PositionId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.UserGuid;
        }

        throw new InvalidOperationException("El email del usuario es requerido.");
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
            throw new KeyNotFoundException("Usuario no encontrado.");

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
            throw new KeyNotFoundException("Usuario no encontrado.");

        return user;
    }

    private string GenerateFolderPath()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 12);
    }
}