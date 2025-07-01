using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.DocumentaryProcedure;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar los procedimientos documentarios del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class DocumentaryProcedureController : Controller
{
    private readonly IDocumentaryProcedureService _documentaryProcedureService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de procedimientos documentarios
    /// </summary>
    /// <param name="documentaryProcedureService">Servicio para gestionar procedimientos documentarios</param>
    public DocumentaryProcedureController(IDocumentaryProcedureService documentaryProcedureService)
    {
        _documentaryProcedureService = documentaryProcedureService;
    }

    /// <summary>
    /// Crea un nuevo procedimiento documentario
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el procedimiento documentario</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(DocumentaryProcedureRequestParams requestParams)
    {
        try
        {
            await _documentaryProcedureService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los procedimientos documentarios disponibles
    /// </summary>
    /// <returns>Lista de procedimientos documentarios</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult<List<DocumentaryProcedureDto>>> GetAllAsync()
    {
        try
        {
            var procedures = await _documentaryProcedureService.GetAllAsync();
            return Ok(procedures);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un procedimiento documentario por su identificador
    /// </summary>
    /// <param name="id">Identificador del procedimiento documentario</param>
    /// <returns>Procedimiento documentario solicitado</returns>
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult<DocumentaryProcedureDto>> GetByIdAsync(int id)
    {
        try
        {
            var procedure = await _documentaryProcedureService.GetByIdAsync(id);
            return Ok(procedure);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un procedimiento documentario existente
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar el procedimiento documentario</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(DocumentaryProcedureRequestParams requestParams)
    {
        try
        {
            await _documentaryProcedureService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un procedimiento documentario por su identificador
    /// </summary>
    /// <param name="requestParams">Parámetros con el identificador del procedimiento documentario a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            await _documentaryProcedureService.DeleteAsync(requestParams.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}