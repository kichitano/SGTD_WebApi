using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Area;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de áreas organizacionales del sistema.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AreaController : Controller
{
    private readonly IAreaService _areaService;

    public AreaController(IAreaService areaService)
    {
        _areaService = areaService;
    }

    /// <summary>
    /// Crea una nueva área en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros de la nueva área a crear.</param>
    /// <returns>Resultado de la operación de creación.</returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(AreaRequestParams requestParams)
    {
        try
        {
            await _areaService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza una área existente en el sistema.
    /// </summary>
    /// <param name="requestParams">Parámetros de la área a actualizar.</param>
    /// <returns>Resultado de la operación de actualización.</returns>
    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(AreaRequestParams requestParams)
    {
        try
        {
            await _areaService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las áreas registradas en el sistema.
    /// </summary>
    /// <returns>Lista de todas las áreas disponibles.</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _areaService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene una área específica por su identificador.
    /// </summary>
    /// <param name="id">Identificador único del área.</param>
    /// <returns>Datos de la área solicitada.</returns>
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _areaService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina una área específica del sistema.
    /// </summary>
    /// <remarks>
    /// Cambios:
    /// <para>2025-07-01 - Christian Cespedes Medina - Ruta de controlador creada.</para>
    /// </remarks>
    /// <param name="requestParams">Parámetros de eliminación que contienen el ID del área.</param>
    /// <returns>Resultado de la operación de eliminación.</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            await _areaService.DeleteByIdAsync(requestParams.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}