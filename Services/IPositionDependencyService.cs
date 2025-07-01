using SGTD_WebApi.Models.PositionDependency;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de dependencias entre posiciones.
/// Proporciona operaciones CRUD para las relaciones jerárquicas entre posiciones organizacionales.
/// </summary>
public interface IPositionDependencyService
{
    /// <summary>
    /// Crea una nueva dependencia entre posiciones de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos de la dependencia a crear</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateAsync(PositionDependencyRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de una dependencia entre posiciones existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos actualizados de la dependencia</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateAsync(PositionDependencyRequestParams requestParams);
    /// <summary>
    /// Obtiene todas las dependencias entre posiciones registradas de forma asíncrona.
    /// </summary>
    /// <returns>Lista de objetos PositionDependencyDto con los datos de todas las dependencias</returns>
    Task<List<PositionDependencyDto>> GetAllAsync();
    /// <summary>
    /// Obtiene una dependencia entre posiciones específica por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la dependencia</param>
    /// <returns>Objeto PositionDependencyDto con los datos de la dependencia encontrada</returns>
    Task<PositionDependencyDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina una dependencia entre posiciones por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la dependencia a eliminar</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task DeleteByIdAsync(int id);
}