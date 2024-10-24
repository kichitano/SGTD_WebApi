using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Infraestructure.ServicesClients;

public interface ICountryServiceClient
{
    Task<List<Country>?> GetResponseCountries();
}