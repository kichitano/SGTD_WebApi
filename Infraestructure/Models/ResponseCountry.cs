namespace SGTD_WebApi.Infraestructure.Models;

/// <summary>
/// Representa la respuesta de un país obtenida desde un servicio externo
/// </summary>
public class ResponseCountry
{
    /// <summary>
    /// Código del país
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// Nombre del país
    /// </summary>
    public string Name { get; set; }
}