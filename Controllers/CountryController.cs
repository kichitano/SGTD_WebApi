using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de países en el sistema.
/// </summary>
[Route("[controller]")]
[ApiController]
public class CountryController : Controller
{
    private readonly ICountryService _countryService;

    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    /// <summary>
    /// Obtiene todos los países disponibles en el sistema.
    /// </summary>
    /// <returns>Lista de todos los países registrados.</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _countryService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}