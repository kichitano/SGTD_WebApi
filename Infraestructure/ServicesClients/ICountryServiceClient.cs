using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Infraestructure.ServicesClients;

/// <summary>
/// Define los métodos para interactuar con servicios externos de países
/// </summary>
public interface ICountryServiceClient
{
    /// <summary>
    /// Obtiene la lista de países desde un servicio externo
    /// </summary>
    /// <returns>Lista de países o null si no se pueden obtener</returns>
    Task<List<Country>?> GetResponseCountries();
}