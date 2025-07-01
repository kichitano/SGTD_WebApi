using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.PositionRole;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar las asignaciones de roles a usuarios del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class UserRoleController : Controller
{
    private readonly IUserRoleService _userRoleService;

    /// <summary>
    /// Constructor del controlador de roles de usuario
    /// </summary>
    /// <param name="userRoleService">Servicio para gestionar roles de usuario</param>
    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    /// <summary>
    /// Crea una nueva asignación de rol a usuario
    /// </summary>
    /// <param name="requestParams">Parámetros para crear la asignación de rol</param>
    /// <returns>Resultado de la operación de creación</returns>
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

    /// <summary>
    /// Actualiza una asignación de rol a usuario existente
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar la asignación de rol</param>
    /// <returns>Resultado de la operación de actualización</returns>
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

    /// <summary>
    /// Obtiene todas las asignaciones de roles a usuarios del sistema
    /// </summary>
    /// <returns>Lista de todas las asignaciones de roles</returns>
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

    /// <summary>
    /// Obtiene una asignación de rol específica mediante su ID
    /// </summary>
    /// <param name="id">ID de la asignación de rol a buscar</param>
    /// <returns>Asignación de rol encontrada</returns>
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

    /// <summary>
    /// Elimina todas las asignaciones de roles de un usuario mediante su GUID
    /// </summary>
    /// <param name="requestParams">Parámetros con el GUID del usuario</param>
    /// <returns>Resultado de la operación de eliminación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByUserGuidAsync([FromBody] DeleteByGuidRequestParams requestParams)
    {
        try
        {
            if (requestParams == null || requestParams.Guid == Guid.Empty)
            {
                return BadRequest("GUID de usuario inválido.");
            }

            await _userRoleService.DeleteByUserGuidAsync(requestParams.Guid);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las asignaciones de roles de un usuario específico
    /// </summary>
    /// <param name="userGuid">GUID del usuario para buscar asignaciones</param>
    /// <returns>Lista de asignaciones de roles del usuario especificado</returns>
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