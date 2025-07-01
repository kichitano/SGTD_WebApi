using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de roles.
/// Proporciona operaciones CRUD para la entidad Rol y funcionalidades adicionales.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Crea un nuevo rol de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos del rol a crear</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateAsync(RoleRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de un rol existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos actualizados del rol</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateAsync(RoleRequestParams requestParams);
    /// <summary>
    /// Obtiene todos los roles registrados de forma asíncrona.
    /// </summary>
    /// <returns>Lista de objetos RoleDto con los datos de todos los roles</returns>
    Task<List<RoleDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un rol específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único del rol</param>
    /// <returns>Objeto RoleDto con los datos del rol encontrado</returns>
    Task<RoleDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina un rol por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único del rol a eliminar</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task DeleteByIdAsync(int id);

    /// <summary>
    /// Crea un nuevo rol y retorna su identificador de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos del rol a crear</param>
    /// <returns>Identificador único del rol creado</returns>
    Task<int> CreateReturnIdAsync(RoleRequestParams requestParams);
}