using SGTD_WebApi.Models.AreaDependency;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de dependencias entre áreas.
/// </summary>
public interface IAreaDependencyService
{
    /// <summary>
    /// Crea una nueva dependencia entre áreas.
    /// </summary>
    /// <param name="requestParams">Parámetros de la nueva dependencia.</param>
    Task CreateAsync(AreaDependencyRequestParams requestParams);
    /// <summary>
    /// Actualiza una dependencia existente entre áreas.
    /// </summary>
    /// <param name="requestParams">Parámetros de actualización de la dependencia.</param>
    Task UpdateAsync(AreaDependencyRequestParams requestParams);
    /// <summary>
    /// Obtiene todas las dependencias entre áreas.
    /// </summary>
    /// <returns>Lista de dependencias entre áreas.</returns>
    Task<List<AreaDependencyDto>> GetAllAsync();
    /// <summary>
    /// Obtiene una dependencia entre áreas por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la dependencia.</param>
    /// <returns>Dependencia encontrada.</returns>
    Task<AreaDependencyDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina una dependencia entre áreas.
    /// </summary>
    /// <param name="id">Identificador de la dependencia a eliminar.</param>
    Task DeleteByIdAsync(int id);
}