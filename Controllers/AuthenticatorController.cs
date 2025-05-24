using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Authenticator;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticatorController : Controller
{
    private readonly IAuthenticatorService _authenticatorService;
    private readonly IAuthenticatorEmailService _authenticatorEmailService;
    private readonly IAuthService _authService;

    public AuthenticatorController(
        IAuthenticatorEmailService authenticatorEmailService, 
        IAuthenticatorService authenticatorService, 
        IAuthService authService)
    {
        _authenticatorEmailService = authenticatorEmailService;
        _authenticatorService = authenticatorService;
        _authService = authService;
    }

    [HttpPost("send-authenticator-email")]
    public async Task<ActionResult> SendAuthenticatorEmail([FromBody] AuthenticatorEmailRequestParams requestParams)
    {
        try
        {
            await _authenticatorEmailService.SendAuthenticatorEmailAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("activate-authenticator-token")]
    public async Task<ActionResult> ActivateAuthenticatorToken(string token)
    {
        try
        {
            var response = await _authenticatorService.ActivateAuthenticatorToken(token);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("verify-authenticator-otp")]
    public async Task<ActionResult> VerifyAuthenticatorOtp([FromBody] AuthenticatorOtpRequestParams requestParams)
    {
        try
        {
            // Only for test purposes
            bool isValid;
            if (requestParams.Email.Equals("test@test.com"))
            {
                isValid = true;
            }
            else
            {
                isValid = await _authenticatorService.VerifyAuthenticatorOtpAsync(requestParams);
            }
            // End of test purposes

            //var isValid = await _authenticatorService.VerifyAuthenticatorOtpAsync(requestParams);

            if (!isValid) return BadRequest(new { message = "Código OTP inválido." });
            var response = await _authService.LoginOtpAsync(requestParams);
            if (!response.Success) return BadRequest(new { message = "Credenciales inválidas." });
            var cookieOptions = _authService.SetRefreshTokenCookie(response.RefreshToken);
            Response.Cookies.Append("refresh_token", response.RefreshToken, cookieOptions);
            return Ok(response);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        } 
    }
}