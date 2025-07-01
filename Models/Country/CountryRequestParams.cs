namespace SGTD_WebApi.Models.Country;

/// <summary>
/// Parámetros de solicitud para operaciones de países
/// </summary>
public class CountryRequestParams
{
    public string Code { get; set; }
    public string Name { get; set; }
}