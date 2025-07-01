using SGTD_WebApi.Models.User;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de usuarios.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros del nuevo usuario.</param>
    Task CreateAsync(UserRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    /// <param name="requestParams">Parámetros de actualización del usuario.</param>
    Task UpdateAsync(UserRequestParams requestParams);
    /// <summary>
    /// Obtiene todos los usuarios del sistema.
    /// </summary>
    /// <returns>Lista de usuarios.</returns>
    Task<List<UserDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un usuario por su identificador.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <returns>Usuario encontrado.</returns>
    Task<UserDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina un usuario usando su GUID.
    /// </summary>
    /// <param name="requestParams">Parámetros de eliminación del usuario.</param>
    Task DeleteByGuidAsync(UserDeletedRequestParams requestParams);
    /// <summary>
    /// Crea un nuevo usuario y retorna su GUID.
    /// </summary>
    /// <param name="requestParams">Parámetros del nuevo usuario.</param>
    /// <returns>GUID del usuario creado.</returns>
    Task<Guid> CreateReturnGuidAsync(UserRequestParams requestParams);
    /// <summary>
    /// Obtiene un usuario por su GUID.
    /// </summary>
    /// <param name="guid">GUID del usuario.</param>
    /// <returns>Usuario encontrado.</returns>
    Task<UserDto> GetByGuidAsync(Guid guid);
    /// <summary>
    /// Obtiene el ID de un usuario por su GUID.
    /// </summary>
    /// <param name="guid">GUID del usuario.</param>
    /// <returns>Datos del usuario con su ID.</returns>
    Task<UserDto> GetIdByGuidAsync(Guid guid);

}