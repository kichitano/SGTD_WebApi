using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.DocumentType;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar los tipos de documentos del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class DocumentTypeController : Controller
{
    private readonly IDocumentTypeService _documentTypeService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de tipos de documentos
    /// </summary>
    /// <param name="documentTypeService">Servicio para gestionar tipos de documentos</param>
    public DocumentTypeController(IDocumentTypeService documentTypeService)
    {
        _documentTypeService = documentTypeService;
    }

    /// <summary>
    /// Crea un nuevo tipo de documento
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el tipo de documento</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(DocumentTypeRequestParams requestParams)
    {
        try
        {
            await _documentTypeService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un tipo de documento existente
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar el tipo de documento</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(DocumentTypeRequestParams requestParams)
    {
        try
        {
            await _documentTypeService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de documentos disponibles
    /// </summary>
    /// <returns>Lista de tipos de documentos</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _documentTypeService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un tipo de documento por su identificador
    /// </summary>
    /// <param name="id">Identificador del tipo de documento</param>
    /// <returns>Tipo de documento solicitado</returns>
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _documentTypeService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un tipo de documento por su identificador
    /// </summary>
    /// <param name="requestParams">Parámetros con el identificador del tipo de documento a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            await _documentTypeService.DeleteByIdAsync(requestParams.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}