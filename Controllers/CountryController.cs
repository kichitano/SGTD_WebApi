using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class CountryController : Controller
{
    private readonly ICountryService _countryService;

    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }

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
            return StatusCode(500, ex.Message);
        }
    }
}