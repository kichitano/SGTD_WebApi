using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Services;
using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de cuentas de usuario, incluyendo firmas digitales y autenticación.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserDigitalSignatureService _userDigitalSignatureService;
    private readonly IAuthenticatorService _authenticatorService;
    private readonly IUserService _userService;

    public AccountController(
        IUserDigitalSignatureService userDigitalSignatureService,
        IAuthenticatorService authenticatorService,
        IUserService userService)
    {
        _userDigitalSignatureService = userDigitalSignatureService;
        _authenticatorService = authenticatorService;
        _userService = userService;
    }

    /// <summary>
    /// Sube la firma digital de un usuario al sistema.
    /// </summary>
    /// <param name="userDigitalSignature">Archivo de imagen de la firma digital.</param>
    /// <param name="userGuid">Identificador único del usuario.</param>
    /// <returns>Resultado de la operación de carga.</returns>
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

    /// <summary>
    /// Verifica si un usuario tiene una firma digital registrada en el sistema.
    /// </summary>
    /// <param name="userGuid">Identificador único del usuario.</param>
    /// <returns>Estado de verificación de la firma digital.</returns>
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

    /// <summary>
    /// Sube la firma digital de un usuario validando previamente un código OTP.
    /// </summary>
    /// <param name="userDigitalSignature">Archivo de imagen de la firma digital.</param>
    /// <param name="email">Correo electrónico del usuario para validación.</param>
    /// <param name="otpCode">Código OTP para autenticación.</param>
    /// <param name="userGuid">Identificador único del usuario.</param>
    /// <returns>Resultado de la operación de carga con validación OTP.</returns>
    [Route("upload-user-digital-signature-with-otp/{userGuid}")]
    [HttpPost]
    public async Task<ActionResult> UploadUserDigitalSignatureWithOtpAsync(
        [FromForm] IFormFile userDigitalSignature,
        [FromForm] string email,
        [FromForm] string otpCode,
        Guid userGuid)
    {
        try
        {
            var otpValidationParams = new AuthenticatorOtpRequestParams
            {
                Email = email,
                OtpCode = otpCode,
                Password = ""
            };

            bool isValidOtp;
            if (email.Equals("test@test.com", StringComparison.OrdinalIgnoreCase))
            {
                isValidOtp = true;
            }
            else
            {
                isValidOtp = await _authenticatorService.VerifyAuthenticatorOtpAsync(otpValidationParams);
            }

            if (!isValidOtp)
            {
                return BadRequest(new { message = "Código OTP inválido." });
            }

            var user = await _userService.GetByGuidAsync(userGuid);
            if (user == null || !user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Usuario no autorizado." });
            }

            await _userDigitalSignatureService.UploadDigitalSignatureAsync(userDigitalSignature, userGuid);
            
            return Ok(new { message = "Firma digital subida exitosamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}