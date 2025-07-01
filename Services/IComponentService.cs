using SGTD_WebApi.Models.Component;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de componentes del sistema.
/// </summary>
public interface IComponentService
{
    /// <summary>
    /// Crea un nuevo componente en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros del nuevo componente.</param>
    Task CreateAsync(ComponentRequestParams requestParams);
    /// <summary>
    /// Actualiza un componente existente.
    /// </summary>
    /// <param name="requestParams">Parámetros de actualización del componente.</param>
    Task UpdateAsync(ComponentRequestParams requestParams);
    /// <summary>
    /// Obtiene todos los componentes del sistema.
    /// </summary>
    /// <returns>Lista de componentes.</returns>
    Task<List<ComponentDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un componente por su identificador.
    /// </summary>
    /// <param name="id">Identificador del componente.</param>
    /// <returns>Componente encontrado.</returns>
    Task<ComponentDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina un componente del sistema.
    /// </summary>
    /// <param name="id">Identificador del componente a eliminar.</param>
    Task DeleteByIdAsync(int id);
}