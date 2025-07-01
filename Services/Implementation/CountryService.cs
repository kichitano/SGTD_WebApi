using AutoMapper;
using SGTD_WebApi.Infraestructure.ServicesClients;
using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar países utilizando un cliente externo.
/// Proporciona funcionalidades para consultar países desde una fuente externa de datos.
/// </summary>
public class CountryService : ICountryService
{
    private readonly ICountryServiceClient _countriesServiceClient;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de países.
    /// </summary>
    /// <param name="countriesServiceClient">Cliente para acceder a los datos de países externos.</param>
    /// <param name="mapper">Mapeador para convertir entre modelos externos e internos.</param>
    public CountryService(ICountryServiceClient countriesServiceClient, IMapper mapper)
    {
        _countriesServiceClient = countriesServiceClient;
        _mapper = mapper;
    }

    /// <summary>
    /// Crea un nuevo país de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del país a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="NotImplementedException">Esta funcionalidad no está implementada.</exception>
    public Task CreateAsync(CountryRequestParams requestParams)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Actualiza un país existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del país.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="NotImplementedException">Esta funcionalidad no está implementada.</exception>
    public Task UpdateAsync(CountryRequestParams requestParams)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Obtiene todos los países desde el cliente externo de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todos los países.</returns>
    public async Task<List<CountryDto>> GetAllAsync()
    {
        var countries = await _countriesServiceClient.GetResponseCountries();
        return _mapper.Map<List<CountryDto>>(countries);
    }

    /// <summary>
    /// Obtiene un país específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del país.</param>
    /// <returns>Un objeto DTO que representa el país.</returns>
    /// <exception cref="NotImplementedException">Esta funcionalidad no está implementada.</exception>
    public Task<CountryDto> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Elimina un país por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del país a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="NotImplementedException">Esta funcionalidad no está implementada.</exception>
    public Task DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}