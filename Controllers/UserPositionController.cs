using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.UserPosition;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class UserPositionController : Controller
{
    private readonly IUserPositionService _userPositionService;

    public UserPositionController(IUserPositionService userPositionService)
    {
        _userPositionService = userPositionService;
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(UserPositionRequestParams requestParams)
    {
        try
        {
            await _userPositionService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(UserPositionRequestParams requestParams)
    {
        try
        {
            await _userPositionService.UpdateAsync(requestParams);
            return Ok();
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
            var response = await _userPositionService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _userPositionService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteByIdAsync(int id)
    {
        try
        {
            await _userPositionService.DeleteByIdAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}