using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Position;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class PositionController : Controller
{
    private readonly IPositionService _positionService;

    public PositionController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(PositionRequestParams requestParams)
    {
        try
        {
            await _positionService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(PositionRequestParams requestParams)
    {
        try
        {
            await _positionService.UpdateAsync(requestParams);
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
            var response = await _positionService.GetAllAsync();
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
            var response = await _positionService.GetByIdAsync(id);
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
            await _positionService.DeleteByIdAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("return")]
    [HttpPost]
    public async Task<ActionResult> CreateReturnIdAsync(PositionRequestParams requestParams)
    {
        try
        {
            var response = await _positionService.CreateReturnIdAsync(requestParams);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("area/{areaId}")]
    [HttpGet]
    public async Task<ActionResult> GetAllByAreaIdAsync(int areaId)
    {
        try
        {
            var response = await _positionService.GetAllByAreaIdAsync(areaId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}