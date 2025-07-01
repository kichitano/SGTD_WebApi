using SGTD_WebApi.Models.Permission;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de permisos.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Crea un nuevo permiso en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros del nuevo permiso.</param>
    Task CreateAsync(PermissionRequestParams requestParams);
    /// <summary>
    /// Actualiza un permiso existente.
    /// </summary>
    /// <param name="requestParams">Parámetros de actualización del permiso.</param>
    Task UpdateAsync(PermissionRequestParams requestParams);
    /// <summary>
    /// Obtiene todos los permisos del sistema.
    /// </summary>
    /// <returns>Lista de permisos.</returns>
    Task<List<PermissionDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un permiso por su identificador.
    /// </summary>
    /// <param name="id">Identificador del permiso.</param>
    /// <returns>Permiso encontrado.</returns>
    Task<PermissionDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina un permiso del sistema.
    /// </summary>
    /// <param name="id">Identificador del permiso a eliminar.</param>
    Task DeleteByIdAsync(int id);
}