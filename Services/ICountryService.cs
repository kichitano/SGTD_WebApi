using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de países.
/// </summary>
public interface ICountryService
{
    /// <summary>
    /// Crea un nuevo país en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros del nuevo país.</param>
    Task CreateAsync(CountryRequestParams requestParams);
    /// <summary>
    /// Actualiza los datos de un país existente.
    /// </summary>
    /// <param name="requestParams">Parámetros de actualización del país.</param>
    Task UpdateAsync(CountryRequestParams requestParams);
    /// <summary>
    /// Obtiene todos los países del sistema.
    /// </summary>
    /// <returns>Lista de países.</returns>
    Task<List<CountryDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un país por su identificador.
    /// </summary>
    /// <param name="id">Identificador del país.</param>
    /// <returns>País encontrado.</returns>
    Task<CountryDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina un país del sistema.
    /// </summary>
    /// <param name="id">Identificador del país a eliminar.</param>
    Task DeleteByIdAsync(int id);
}