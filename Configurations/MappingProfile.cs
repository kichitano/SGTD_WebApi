using AutoMapper;
using SGTD_WebApi.Models.Country;

namespace SGTD_WebApi.Configurations;

/// <summary>
/// Perfil de configuración de AutoMapper que define los mapeos entre entidades y DTOs del sistema
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Inicializa una nueva instancia del perfil de mapeo y configura las conversiones entre objetos
    /// </summary>
    public MappingProfile()
    {
        CreateMap<Country, CountryDto>();
    }
}