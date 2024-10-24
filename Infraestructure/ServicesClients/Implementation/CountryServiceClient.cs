using AutoMapper;
using SGTD_WebApi.Infraestructure.Models;
using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Infraestructure.ServicesClients.Implementation;

public class CountryServiceClient : ICountryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiGetCountries = "Countries:CountriesApiURL";
    private readonly IMapper _mapper;

    public CountryServiceClient(HttpClient httpClient, IMapper mapper, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _mapper = ConfigureMapping();
        _httpClient.BaseAddress = new Uri(configuration[_apiGetCountries] ?? string.Empty);
    }

    protected static Mapper ConfigureMapping()
    {
        return new(new MapperConfiguration(conf =>
        {
            conf.CreateMap<ResponseCountry, Country>()
                .ForMember(d => d.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name));
        }));
    }

    public async Task<List<Country>?> GetResponseCountries()
    {
        var response = await _httpClient.GetAsync("/names.json");
        if (!response.IsSuccessStatusCode)
        {
            return new List<Country>();
        }

        var countriesData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (countriesData == null || !countriesData.Any())
        {
            return new List<Country>();
        }

        var countriesList = countriesData.Select(kvp => new ResponseCountry
        {
            Code = kvp.Key,
            Name = kvp.Value
        }).ToList();

        return _mapper.Map<List<ResponseCountry>, List<Country>>(countriesList);
    }
}