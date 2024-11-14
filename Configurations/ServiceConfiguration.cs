using SGTD_WebApi.Infraestructure.ServicesClients;
using SGTD_WebApi.Infraestructure.ServicesClients.Implementation;
using SGTD_WebApi.Services;
using SGTD_WebApi.Services.Implementation;

namespace SGTD_WebApi.Configurations;

public static class ServiceConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICountryServiceClient, CountryServiceClient>();

        services.AddScoped<IAreaDependencyService, AreaDependencyService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IComponentService, ComponentService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IPositionDependencyService, PositionDependencyService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleComponentPermissionService, RoleComponentPermissionService>();
        services.AddScoped<IUserPositionService, UserPositionService>();
        services.AddScoped<IUserService, UserService>();

        services.AddAutoMapper(typeof(MappingProfile));
    }
}