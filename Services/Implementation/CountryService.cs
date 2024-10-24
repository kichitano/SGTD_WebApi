using AutoMapper;
using SGTD_WebApi.Infraestructure.ServicesClients;
using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Services.Implementation;

public class CountryService : ICountryService
{
    private readonly ICountryServiceClient _countriesServiceClient;
    private readonly IMapper _mapper;

    public CountryService(ICountryServiceClient countriesServiceClient, IMapper mapper)
    {
        _countriesServiceClient = countriesServiceClient;
        _mapper = mapper;
    }

    public Task CreateAsync(CountryRequestParams requestParams)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CountryRequestParams requestParams)
    {
        throw new NotImplementedException();
    }

    public async Task<List<CountryDto>> GetAllAsync()
    {
        var countries = await _countriesServiceClient.GetResponseCountries();
        return _mapper.Map<List<CountryDto>>(countries);
    }

    public Task<CountryDto> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}