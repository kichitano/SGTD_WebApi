using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Position;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar los puestos de trabajo del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class PositionController : Controller
{
    private readonly IPositionService _positionService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de puestos
    /// </summary>
    /// <param name="positionService">Servicio para gestionar puestos</param>
    public PositionController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    /// <summary>
    /// Crea un nuevo puesto de trabajo
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el puesto</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(PositionRequestParams requestParams)
    {
        try
        {
            await _positionService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un puesto de trabajo existente
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar el puesto</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(PositionRequestParams requestParams)
    {
        try
        {
            await _positionService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los puestos de trabajo disponibles
    /// </summary>
    /// <returns>Lista de puestos</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _positionService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un puesto de trabajo por su identificador
    /// </summary>
    /// <param name="id">Identificador del puesto</param>
    /// <returns>Puesto solicitado</returns>
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _positionService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un puesto de trabajo por su identificador
    /// </summary>
    /// <param name="requestParams">Parámetros con el identificador del puesto a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            if (requestParams == null || requestParams.Id <= 0)
            {
                return BadRequest("ID de posición inválido.");
            }

            await _positionService.DeleteByIdAsync(requestParams.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Crea un nuevo puesto de trabajo y retorna su identificador
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el puesto</param>
    /// <returns>Identificador del puesto creado</returns>
    [Route("return")]
    [HttpPost]
    public async Task<ActionResult> CreateReturnIdAsync(PositionRequestParams requestParams)
    {
        try
        {
            var response = await _positionService.CreateReturnIdAsync(requestParams);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los puestos de trabajo de un área específica
    /// </summary>
    /// <param name="areaId">Identificador del área</param>
    /// <returns>Lista de puestos del área</returns>
    [Route("area/{areaId}")]
    [HttpGet]
    public async Task<ActionResult> GetAllByAreaIdAsync(int areaId)
    {
        try
        {
            var response = await _positionService.GetAllByAreaIdAsync(areaId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene los jefes directos disponibles para un área, excluyendo opcionalmente un puesto
    /// </summary>
    /// <param name="currentAreaId">Identificador del área actual</param>
    /// <param name="excludePositionId">Identificador del puesto a excluir (opcional)</param>
    /// <returns>Lista de jefes directos disponibles</returns>
    [Route("available-managers/{currentAreaId}")]
    [HttpGet]
    public async Task<ActionResult> GetAvailableDirectManagersAsync(int currentAreaId, [FromQuery] int? excludePositionId = null)
    {
        try
        {
            var response = await _positionService.GetAvailableDirectManagersAsync(currentAreaId, excludePositionId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Verifica si un área tiene la máxima autoridad disponible
    /// </summary>
    /// <param name="areaId">Identificador del área</param>
    /// <param name="excludePositionId">Identificador del puesto a excluir (opcional)</param>
    /// <returns>Indicador de si el área tiene máxima autoridad</returns>
    [Route("area/{areaId}/has-max-authority")]
    [HttpGet]
    public async Task<ActionResult> AreaHasMaxAuthorityAsync(int areaId, [FromQuery] int? excludePositionId = null)
    {
        try
        {
            var response = await _positionService.AreaHasMaxAuthorityAsync(areaId, excludePositionId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene el puesto con máxima autoridad en un área específica
    /// </summary>
    /// <param name="areaId">Identificador del área</param>
    /// <param name="excludePositionId">Identificador del puesto a excluir (opcional)</param>
    /// <returns>Puesto con máxima autoridad en el área</returns>
    [Route("area/{areaId}/max-authority")]
    [HttpGet]
    public async Task<ActionResult> GetMaxAuthorityByAreaAsync(int areaId, [FromQuery] int? excludePositionId = null)
    {
        try
        {
            var response = await _positionService.GetMaxAuthorityByAreaAsync(areaId, excludePositionId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}