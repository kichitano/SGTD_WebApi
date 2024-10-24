using AutoMapper;
using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Configurations;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Country, CountryDto>();
    }
}