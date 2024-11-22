using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Auth;
using SGTD_WebApi.Services;
using SGTD_WebApi.Services.Implementation;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserTokenService _userTokenService;

    public AuthController(IAuthService authService, IUserTokenService userTokenService)
    {
        _authService = authService;
        _userTokenService = userTokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync(AuthRequestParams requestParams)
    {
        var response = await _authService.LoginAsync(requestParams);
        if (response.Success)
        {
            var cookieOptions = _authService.SetRefreshTokenCookie(response.RefreshToken);
            Response.Cookies.Append("refresh_token", response.RefreshToken, cookieOptions);
            return Ok(response);
        }
        return Unauthorized(new { message = "Invalid credentials" });
    }

    [HttpPost("logout")]
    public async Task<ActionResult> LogoutAsync(LogoutRequestParams requestParams)
    {
        Response.Cookies.Delete("refresh_token");
        await _authService.LogoutAsync(requestParams);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshTokenAsync()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "No refresh token found" });
        }
        var userGuid = await _userTokenService.GetUserGuidFromRefreshTokenAsync(refreshToken);
        if (userGuid == null)
        {
            return Unauthorized(new { message = "Invalid refresh token" });
        }
        var newAccessToken = await _userTokenService.GenerateTokenAsync(userGuid.Value);
        var newRefreshToken = await _userTokenService.GetUserRefreshTokenFromGeneratedTokenAsync(newAccessToken);
        var cookieOptions = _authService.SetRefreshTokenCookie(newRefreshToken);
        Response.Cookies.Append("refresh_token", newRefreshToken, cookieOptions);
        return Ok(new AuthDto{ Success = true, Token = newAccessToken });
    }

    [HttpPost("validate-token")]
    public async Task<ActionResult> ValidateTokenAsync(string token)
    {
        var isValid = await _authService.ValidateTokenAsync(token);
        if (!isValid)
        {
            return Unauthorized();
        }
        return Ok();
    }
}