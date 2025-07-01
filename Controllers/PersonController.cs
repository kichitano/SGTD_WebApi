using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Person;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar las personas del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class PersonController : Controller
{
    private readonly IPersonService _personService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de personas
    /// </summary>
    /// <param name="personService">Servicio para gestionar personas</param>
    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Crea una nueva persona
    /// </summary>
    /// <param name="requestParams">Parámetros para crear la persona</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(PersonRequestParams requestParams)
    {
        try
        {
            await _personService.CreateAsync(requestParams);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza una persona existente
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar la persona</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(PersonRequestParams requestParams)
    {
        try
        {
            await _personService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las personas disponibles
    /// </summary>
    /// <returns>Lista de personas</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync()
    {
        try
        {
            var response = await _personService.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene una persona por su identificador
    /// </summary>
    /// <param name="id">Identificador de la persona</param>
    /// <returns>Persona solicitada</returns>
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _personService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina una persona por su identificador
    /// </summary>
    /// <param name="requestParams">Parámetros con el identificador de la persona a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            await _personService.DeleteByIdAsync(requestParams.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}