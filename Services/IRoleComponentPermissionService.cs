using SGTD_WebApi.Models.RoleComponentPermission;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de permisos de componentes por rol.
/// Proporciona operaciones CRUD para las relaciones entre roles, componentes y permisos.
/// </summary>
public interface IRoleComponentPermissionService
{
    /// <summary>
    /// Crea un nuevo permiso de componente para un rol de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos del permiso a crear</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateAsync(RoleComponentPermissionRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de un permiso de componente existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos actualizados del permiso</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateAsync(RoleComponentPermissionRequestParams requestParams);
    /// <summary>
    /// Obtiene todos los permisos de componentes por rol registrados de forma asíncrona.
    /// </summary>
    /// <returns>Lista de objetos RoleComponentPermissionDto con todos los permisos</returns>
    Task<List<RoleComponentPermissionDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un permiso de componente específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único del permiso</param>
    /// <returns>Objeto RoleComponentPermissionDto con los datos del permiso encontrado</returns>
    Task<RoleComponentPermissionDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina un permiso de componente por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único del permiso a eliminar</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task DeleteByIdAsync(int id);

    /// <summary>
    /// Crea múltiples permisos de componentes para roles de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Array de parámetros de solicitud con los datos de los permisos a crear</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateArrayAsync(RoleComponentPermissionRequestParams[] requestParams);
    /// <summary>
    /// Actualiza múltiples permisos de componentes para un rol específico de forma asíncrona.
    /// </summary>
    /// <param name="roleId">Identificador del rol</param>
    /// <param name="requestParams">Array de parámetros de solicitud con los datos actualizados de los permisos</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateArrayAsync(int roleId, RoleComponentPermissionRequestParams[] requestParams);
    /// <summary>
    /// Obtiene todos los permisos de componentes asociados a un rol específico de forma asíncrona.
    /// </summary>
    /// <param name="roleId">Identificador del rol</param>
    /// <returns>Lista de objetos RoleComponentPermissionDto con los permisos del rol especificado</returns>
    Task<List<RoleComponentPermissionDto>> GetByRoleIdAsync(int roleId);
}