using SGTD_WebApi.Models.PositionRole;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de roles de usuarios.
/// Proporciona operaciones CRUD para las asignaciones de roles a usuarios.
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Crea una nueva asignación de rol a un usuario de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos de la asignación a crear</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateAsync(UserRoleRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de una asignación de rol existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos actualizados de la asignación</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateAsync(UserRoleRequestParams requestParams);
    /// <summary>
    /// Obtiene todas las asignaciones de roles a usuarios de forma asíncrona.
    /// </summary>
    /// <returns>Lista de objetos UserRoleDto con todas las asignaciones de roles</returns>
    Task<List<UserRoleDto>> GetAllAsync();
    /// <summary>
    /// Obtiene una asignación de rol específica por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la asignación</param>
    /// <returns>Objeto UserRoleDto con los datos de la asignación encontrada</returns>
    Task<UserRoleDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina todas las asignaciones de roles de un usuario específico de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task DeleteByUserGuidAsync(Guid userGuid);
    /// <summary>
    /// Obtiene todas las asignaciones de roles de un usuario específico de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>Lista de objetos UserRoleDto con las asignaciones de roles del usuario</returns>
    Task<List<UserRoleDto>> GetByUserGuidAsync(Guid userGuid);
}