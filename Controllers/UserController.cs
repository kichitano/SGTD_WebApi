using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.User;
using SGTD_WebApi.Services;
using System;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(UserRequestParams requestParams)
    {
        try
        {
            await _userService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(UserRequestParams requestParams)
    {
        try
        {
            await _userService.UpdateAsync(requestParams);
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
            var response = await _userService.GetAllAsync();
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
            await _userService.DeleteByIdAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("return")]
    [HttpPost]
    public async Task<ActionResult> CreateReturnGuidAsync(UserRequestParams requestParams)
    {
        try
        {
            var response = await _userService.CreateReturnGuidAsync(requestParams);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Route("{guid}")]
    [HttpGet]
    public async Task<ActionResult> GetByGuidAsync(Guid guid)
    {
        try
        {
            var response = await _userService.GetByGuidAsync(guid);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}