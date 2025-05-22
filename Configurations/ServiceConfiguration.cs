using SGTD_WebApi.Infraestructure.ServicesClients;
using SGTD_WebApi.Infraestructure.ServicesClients.Implementation;
using SGTD_WebApi.Services;
using SGTD_WebApi.Services.Implementation;

namespace SGTD_WebApi.Configurations;

public static class ServiceConfiguration
{
    /// <summary>
    /// Recibe una colleccion de servicios para inicializar correctamente la inyeccion de dependencias
    /// </summary>
    /// <param name="services"></param>
    public static void Configure(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICountryServiceClient, CountryServiceClient>();

        services.AddScoped<IAreaDependencyService, AreaDependencyService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<IAuthenticatorEmailService, AuthenticatorEmailService>();
        services.AddScoped<IAuthenticatorService, AuthenticatorService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IComponentService, ComponentService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IPositionDependencyService, PositionDependencyService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IRoleComponentPermissionService, RoleComponentPermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserDigitalSignatureService, UserDigitalSignatureService>();
        services.AddScoped<IUserFileService, UserFileService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserTokenService, UserTokenService>();

        services.AddAutoMapper(typeof(MappingProfile));
    }
}