using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.PositionRole;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class UserRoleController : Controller
{
    private readonly IUserRoleService _userRoleService;

    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(UserRoleRequestParams requestParams)
    {
        try
        {
            await _userRoleService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(UserRoleRequestParams requestParams)
    {
        try
        {
            await _userRoleService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _userRoleService.GetAllAsync();
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
            var response = await _userRoleService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{userGuid}")]
    public async Task<ActionResult> DeleteByUserGuidAsync(Guid userGuid)
    {
        try
        {
            await _userRoleService.DeleteByUserGuidAsync(userGuid);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("user/{userGuid}")]
    [HttpGet]
    public async Task<ActionResult> GetByUserGuidAsync(Guid userGuid)
    {
        try
        {
            var response = await _userRoleService.GetByUserGuidAsync(userGuid);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}