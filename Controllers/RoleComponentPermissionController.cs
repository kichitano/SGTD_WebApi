using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.RoleComponentPermission;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class RoleComponentPermissionController : Controller
{
    private readonly IRoleComponentPermissionService _roleComponentPermissionService;

    public RoleComponentPermissionController(IRoleComponentPermissionService roleComponentPermissionService)
    {
        _roleComponentPermissionService = roleComponentPermissionService;
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(RoleComponentPermissionRequestParams requestParams)
    {
        try
        {
            await _roleComponentPermissionService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(RoleComponentPermissionRequestParams requestParams)
    {
        try
        {
            await _roleComponentPermissionService.UpdateAsync(requestParams);
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
            var response = await _roleComponentPermissionService.GetAllAsync();
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
            var response = await _roleComponentPermissionService.GetByIdAsync(id);
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
            await _roleComponentPermissionService.DeleteByIdAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("array")]
    [HttpPost]
    public async Task<ActionResult> CreateArrayAsync(RoleComponentPermissionRequestParams[] requestParams)
    {
        try
        {
            await _roleComponentPermissionService.CreateArrayAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("array/{roleId}")]
    [HttpPut]
    public async Task<ActionResult> UpdateArrayAsync(int roleId, RoleComponentPermissionRequestParams[] requestParams)
    {
        try
        {
            await _roleComponentPermissionService.UpdateArrayAsync(roleId, requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("role/{roleId}")]
    [HttpGet]
    public async Task<ActionResult> GetByRoleIdAsync(int roleId)
    {
        try
        {
            var response = await _roleComponentPermissionService.GetByRoleIdAsync(roleId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}