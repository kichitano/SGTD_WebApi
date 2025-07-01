using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.RoleComponentPermission;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar los permisos de componentes por rol del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class RoleComponentPermissionController : Controller
{
    private readonly IRoleComponentPermissionService _roleComponentPermissionService;

    public RoleComponentPermissionController(IRoleComponentPermissionService roleComponentPermissionService)
    {
        _roleComponentPermissionService = roleComponentPermissionService;
    }

    /// <summary>
    /// Crea un nuevo permiso de componente para un rol
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el permiso de componente</param>
    /// <returns>Resultado de la operación de creación</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un permiso de componente existente para un rol
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar el permiso de componente</param>
    /// <returns>Resultado de la operación de actualización</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los permisos de componentes por rol del sistema
    /// </summary>
    /// <returns>Lista de todos los permisos de componentes por rol</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un permiso de componente por rol específico mediante su ID
    /// </summary>
    /// <param name="id">ID del permiso de componente a buscar</param>
    /// <returns>Permiso de componente encontrado</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un permiso de componente por rol mediante su ID
    /// </summary>
    /// <param name="requestParams">Parámetros con el ID del permiso a eliminar</param>
    /// <returns>Resultado de la operación de eliminación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            await _roleComponentPermissionService.DeleteByIdAsync(requestParams.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Crea múltiples permisos de componentes para roles en una sola operación
    /// </summary>
    /// <param name="requestParams">Array de parámetros para crear múltiples permisos</param>
    /// <returns>Resultado de la operación de creación múltiple</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza múltiples permisos de componentes para un rol específico
    /// </summary>
    /// <param name="roleId">ID del rol para actualizar permisos</param>
    /// <param name="requestParams">Array de parámetros para actualizar múltiples permisos</param>
    /// <returns>Resultado de la operación de actualización múltiple</returns>
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
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los permisos de componentes asignados a un rol específico
    /// </summary>
    /// <param name="roleId">ID del rol para buscar permisos</param>
    /// <returns>Lista de permisos de componentes del rol especificado</returns>
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
            return BadRequest(ex.Message);
        }
    }
}