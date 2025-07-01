using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.User;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar los usuarios del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el usuario</param>
    /// <returns>Resultado de la operación de creación</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un usuario existente del sistema
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar el usuario</param>
    /// <returns>Resultado de la operación de actualización</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los usuarios del sistema
    /// </summary>
    /// <returns>Lista de todos los usuarios</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un usuario del sistema mediante su GUID
    /// </summary>
    /// <param name="requestParams">Parámetros con el GUID del usuario a eliminar</param>
    /// <returns>Resultado de la operación de eliminación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByGuidAsync(UserDeletedRequestParams requestParams)
    {
        try
        {
            await _userService.DeleteByGuidAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Crea un nuevo usuario y retorna su GUID generado
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el usuario</param>
    /// <returns>GUID del usuario creado</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un usuario específico mediante su GUID
    /// </summary>
    /// <param name="guid">GUID del usuario a buscar</param>
    /// <returns>Usuario encontrado</returns>
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
            return BadRequest(ex.Message);
        }
    }
}