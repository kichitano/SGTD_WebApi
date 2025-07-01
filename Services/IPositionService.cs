using SGTD_WebApi.Models.Position;
using SGTD_WebApi.Models.Role;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de posiciones/cargos.
/// Proporciona operaciones CRUD y funcionalidades especializadas para la gestión de posiciones organizacionales.
/// </summary>
public interface IPositionService
{
    /// <summary>
    /// Crea una nueva posición de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos de la posición a crear</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateAsync(PositionRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de una posición existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos actualizados de la posición</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateAsync(PositionRequestParams requestParams);
    /// <summary>
    /// Obtiene todas las posiciones registradas de forma asíncrona.
    /// </summary>
    /// <returns>Lista de objetos PositionDto con los datos de todas las posiciones</returns>
    Task<List<PositionDto>> GetAllAsync();
    /// <summary>
    /// Obtiene una posición específica por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la posición</param>
    /// <returns>Objeto PositionDto con los datos de la posición encontrada</returns>
    Task<PositionDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina una posición por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la posición a eliminar</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task DeleteByIdAsync(int id);

    /// <summary>
    /// Crea una nueva posición y retorna su identificador de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos de la posición a crear</param>
    /// <returns>Identificador único de la posición creada</returns>
    Task<int> CreateReturnIdAsync(PositionRequestParams requestParams);
    /// <summary>
    /// Obtiene todas las posiciones pertenecientes a un área específica de forma asíncrona.
    /// </summary>
    /// <param name="areaId">Identificador del área</param>
    /// <returns>Lista de objetos PositionDto con las posiciones del área especificada</returns>
    Task<List<PositionDto>> GetAllByAreaIdAsync(int areaId);
    /// <summary>
    /// Obtiene las posiciones disponibles para ser supervisores directos en un área de forma asíncrona.
    /// </summary>
    /// <param name="currentAreaId">Identificador del área actual</param>
    /// <param name="excludePositionId">Identificador de posición a excluir (opcional)</param>
    /// <returns>Lista de objetos PositionDto con las posiciones disponibles como supervisores</returns>
    Task<List<PositionDto>> GetAvailableDirectManagersAsync(int currentAreaId, int? excludePositionId = null);
    /// <summary>
    /// Verifica si un área tiene una posición con máxima autoridad de forma asíncrona.
    /// </summary>
    /// <param name="areaId">Identificador del área</param>
    /// <param name="excludePositionId">Identificador de posición a excluir de la verificación (opcional)</param>
    /// <returns>True si el área tiene una posición con máxima autoridad, False en caso contrario</returns>
    Task<bool> AreaHasMaxAuthorityAsync(int areaId, int? excludePositionId = null);
    /// <summary>
    /// Obtiene la posición con máxima autoridad en un área específica de forma asíncrona.
    /// </summary>
    /// <param name="areaId">Identificador del área</param>
    /// <param name="excludePositionId">Identificador de posición a excluir de la búsqueda (opcional)</param>
    /// <returns>Objeto PositionDto con la posición de máxima autoridad o null si no existe</returns>
    Task<PositionDto?> GetMaxAuthorityByAreaAsync(int areaId, int? excludePositionId = null);

}