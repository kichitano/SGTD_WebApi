using SGTD_WebApi.Models.DocumentaryProcedure;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de procedimientos documentarios.
/// Proporciona operaciones CRUD para la entidad Procedimiento Documentario.
/// </summary>
public interface IDocumentaryProcedureService
{
    /// <summary>
    /// Crea un nuevo procedimiento documentario de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos del procedimiento a crear</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateAsync(DocumentaryProcedureRequestParams requestParams);
    /// <summary>
    /// Obtiene todos los procedimientos documentarios registrados de forma asíncrona.
    /// </summary>
    /// <returns>Lista de objetos DocumentaryProcedureDto con todos los procedimientos</returns>
    Task<List<DocumentaryProcedureDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un procedimiento documentario específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único del procedimiento</param>
    /// <returns>Objeto DocumentaryProcedureDto con los datos del procedimiento encontrado</returns>
    Task<DocumentaryProcedureDto> GetByIdAsync(int id);
    /// <summary>
    /// Actualiza los datos de un procedimiento documentario existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos actualizados del procedimiento</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateAsync(DocumentaryProcedureRequestParams requestParams);
    /// <summary>
    /// Elimina un procedimiento documentario por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único del procedimiento a eliminar</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task DeleteAsync(int id);
}