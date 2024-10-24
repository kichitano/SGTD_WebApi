using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.AreaDependency;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AreaDependencyController : Controller
{
    private readonly IAreaDependencyService _areaDependencyService;

    public AreaDependencyController(IAreaDependencyService areaDependencyService)
    {
        _areaDependencyService = areaDependencyService;
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(AreaDependencyRequestParams requestParams)
    {
        try
        {
            await _areaDependencyService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(AreaDependencyRequestParams requestParams)
    {
        try
        {
            await _areaDependencyService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _areaDependencyService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _areaDependencyService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteByIdAsync(int id)
    {
        try
        {
            await _areaDependencyService.DeleteByIdAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}