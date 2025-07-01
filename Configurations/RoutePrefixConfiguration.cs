using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SGTD_WebApi.Configurations;

/// <summary>
/// Configuración de convención para agregar un prefijo común a todas las rutas de los controladores de la aplicación
/// </summary>
/// <param name="prefix">Prefijo que se agregará a todas las rutas de los controladores</param>
public class RoutePrefixConfiguration(string prefix) : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix = new(new RouteAttribute(prefix));

    /// <summary>
    /// Aplica el prefijo de ruta configurado a todos los controladores de la aplicación
    /// </summary>
    /// <param name="application">Modelo de aplicación que contiene la información de todos los controladores</param>
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                selector.AttributeRouteModel = selector.AttributeRouteModel != null 
                    ? AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel) 
                    : _routePrefix;
            }
        }
    }
}