using SGTD_WebApi.Models.Person;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de personas.
/// Proporciona operaciones CRUD para la entidad Persona.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Crea una nueva persona de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos de la persona a crear</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateAsync(PersonRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de una persona existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos actualizados de la persona</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateAsync(PersonRequestParams requestParams);
    /// <summary>
    /// Obtiene todas las personas registradas de forma asíncrona.
    /// </summary>
    /// <returns>Lista de objetos PersonDto con los datos de todas las personas</returns>
    Task<List<PersonDto>> GetAllAsync();
    /// <summary>
    /// Obtiene una persona específica por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la persona</param>
    /// <returns>Objeto PersonDto con los datos de la persona encontrada</returns>
    Task<PersonDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina una persona por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la persona a eliminar</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task DeleteByIdAsync(int id);
}