using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Permission;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar los permisos del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class PermissionController : Controller
{
    private readonly IPermissionService _permissionService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de permisos
    /// </summary>
    /// <param name="permissionService">Servicio para gestionar permisos</param>
    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Crea un nuevo permiso
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el permiso</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(PermissionRequestParams requestParams)
    {
        try
        {
            await _permissionService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un permiso existente
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar el permiso</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(PermissionRequestParams requestParams)
    {
        try
        {
            await _permissionService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los permisos disponibles
    /// </summary>
    /// <returns>Lista de permisos</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _permissionService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un permiso por su identificador
    /// </summary>
    /// <param name="id">Identificador del permiso</param>
    /// <returns>Permiso solicitado</returns>
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _permissionService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un permiso por su identificador
    /// </summary>
    /// <param name="requestParams">Parámetros con el identificador del permiso a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            await _permissionService.DeleteByIdAsync(requestParams.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}