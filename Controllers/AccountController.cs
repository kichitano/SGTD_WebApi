using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserDigitalSignatureService _userDigitalSignatureService;

    public AccountController(IUserDigitalSignatureService userDigitalSignatureService)
    {
        _userDigitalSignatureService = userDigitalSignatureService;
    }

    [Route("upload-user-digital-signature/{userGuid}")]
    [HttpPost]
    public async Task<ActionResult> UploadUserDigitialSignatureAsync([FromForm] IFormFile userDigitalSignature, Guid userGuid)
    {
        try
        {
            await _userDigitalSignatureService.UploadDigitalSignatureAsync(userDigitalSignature, userGuid);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("verify-user-digital-signature/{userGuid}")]
    [HttpGet]
    public async Task<ActionResult> VerifyUserDigitalSignatureAsync(Guid userGuid)
    {
        try
        {
            var response = await _userDigitalSignatureService.VerifyUserDigitalSignatureAsync(userGuid);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}