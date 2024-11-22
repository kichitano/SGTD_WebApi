using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Area;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AreaController : Controller
{
    private readonly IAreaService _areaService;

    public AreaController(IAreaService areaService)
    {
        _areaService = areaService;
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(AreaRequestParams requestParams)
    {
        try
        {
            await _areaService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(AreaRequestParams requestParams)
    {
        try
        {
            await _areaService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _areaService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _areaService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteByIdAsync(int id)
    {
        try
        {
            await _areaService.DeleteByIdAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}