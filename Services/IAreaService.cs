using SGTD_WebApi.Models.Area;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de áreas.
/// </summary>
public interface IAreaService
{
    /// <summary>
    /// Crea una nueva área en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros de la nueva área.</param>
    Task CreateAsync(AreaRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de un área existente.
    /// </summary>
    /// <param name="requestParams">Parámetros de actualización del área.</param>
    Task UpdateAsync(AreaRequestParams requestParams);
    /// <summary>
    /// Obtiene todas las áreas del sistema.
    /// </summary>
    /// <returns>Lista de áreas.</returns>
    Task<List<AreaDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un área por su identificador.
    /// </summary>
    /// <param name="id">Identificador del área.</param>
    /// <returns>Área encontrada.</returns>
    Task<AreaDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina un área del sistema.
    /// </summary>
    /// <param name="id">Identificador del área a eliminar.</param>
    Task DeleteByIdAsync(int id);
}