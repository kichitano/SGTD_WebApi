using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Component;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de componentes del sistema.
/// </summary>
[Route("[controller]")]
[ApiController]
public class ComponentController : Controller
{
    private readonly IComponentService _componentService;

    public ComponentController(IComponentService componentService)
    {
        _componentService = componentService;
    }

    /// <summary>
    /// Crea un nuevo componente en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros del nuevo componente a crear.</param>
    /// <returns>Resultado de la operación de creación.</returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(ComponentRequestParams requestParams)
    {
        try
        {
            await _componentService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un componente existente en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros del componente a actualizar.</param>
    /// <returns>Resultado de la operación de actualización.</returns>
    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(ComponentRequestParams requestParams)
    {
        try
        {
            await _componentService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los componentes registrados en el sistema.
    /// </summary>
    /// <returns>Lista de todos los componentes disponibles.</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _componentService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un componente específico por su identificador.
    /// </summary>
    /// <param name="id">Identificador único del componente.</param>
    /// <returns>Datos del componente solicitado.</returns>
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _componentService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un componente específico del sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros de eliminación que contienen el ID del componente.</param>
    /// <returns>Resultado de la operación de eliminación.</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            await _componentService.DeleteByIdAsync(requestParams.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}