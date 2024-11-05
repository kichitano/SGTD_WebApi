using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Services;

public interface ICountryService
{
    Task CreateAsync(CountryRequestParams requestParams);
    Task UpdateAsync(CountryRequestParams requestParams);
    Task<List<CountryDto>> GetAllAsync();
    Task<CountryDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}