using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.DocumentaryProcess;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar los procesos documentarios y sus instancias
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize]
public class DocumentaryProcessController : Controller
{
    private readonly IDocumentaryProcessService _documentaryProcessService;
    private readonly IUserService _userService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de procesos documentarios
    /// </summary>
    /// <param name="documentaryProcessService">Servicio para gestionar procesos documentarios</param>
    /// <param name="userService">Servicio para gestionar usuarios</param>
    public DocumentaryProcessController(IDocumentaryProcessService documentaryProcessService, IUserService userService)
    {
        _documentaryProcessService = documentaryProcessService;
        _userService = userService;
    }

    /// <summary>
    /// Obtiene el identificador del usuario actual desde el contexto de autenticación
    /// </summary>
    /// <returns>Identificador del usuario actual</returns>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : 0;
    }

    /// <summary>
    /// Obtiene el identificador de usuario a partir de un GUID o del usuario actual
    /// </summary>
    /// <param name="userGuid">GUID del usuario opcional</param>
    /// <returns>Identificador del usuario</returns>
    private async Task<int> GetUserIdFromGuidAsync(Guid? userGuid)
    {
        if (userGuid.HasValue)
        {
            var user = await _userService.GetByGuidAsync(userGuid.Value);
            return user?.Id ?? 0;
        }
        return GetCurrentUserId();
    }

    /// <summary>
    /// Crea un nuevo proceso documentario
    /// </summary>
    /// <param name="requestParams">Parámetros para crear el proceso documentario</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(DocumentaryProcessRequestParams requestParams)
    {
        try
        {
            var userId = await GetUserIdFromGuidAsync(requestParams.UserGuid);
            await _documentaryProcessService.CreateAsync(requestParams, userId);
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
    /// Actualiza un proceso documentario existente
    /// </summary>
    /// <param name="requestParams">Parámetros para actualizar el proceso documentario</param>
    /// <returns>Resultado de la operación</returns>
    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(DocumentaryProcessRequestParams requestParams)
    {
        try
        {
            var userId = await GetUserIdFromGuidAsync(requestParams.UserGuid);
            await _documentaryProcessService.UpdateAsync(requestParams, userId);
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
    /// Obtiene todos los procesos documentarios disponibles para un usuario
    /// </summary>
    /// <param name="userGuid">GUID del usuario opcional</param>
    /// <returns>Lista de procesos documentarios</returns>
    [Route("")]
    [HttpGet]
    public async Task<ActionResult> GetAllAsync([FromQuery] Guid? userGuid = null)
    {
        try
        {
            var userId = await GetUserIdFromGuidAsync(userGuid);
            var response = await _documentaryProcessService.GetAllAsync(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un proceso documentario por su identificador
    /// </summary>
    /// <param name="requestParams">Parámetros con el identificador del proceso a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [Route("delete")]
    [HttpPost]
    public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _documentaryProcessService.DeleteByIdAsync(requestParams.Id, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    /// <summary>
    /// Obtiene los procesos documentarios creados por el usuario actual
    /// </summary>
    /// <param name="userGuid">GUID del usuario opcional</param>
    /// <returns>Lista de procesos creados por el usuario</returns>
    [HttpGet("my-processes")]
    public async Task<ActionResult<List<DocumentaryProcessInstanceDto>>> GetMyProcesses([FromQuery] Guid? userGuid = null)
    {
        try
        {
            var userId = await GetUserIdFromGuidAsync(userGuid);
            var processes = await _documentaryProcessService.GetMyProcessesAsync(userId);
            return Ok(processes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene los procesos documentarios pendientes para el usuario actual
    /// </summary>
    /// <param name="userGuid">GUID del usuario opcional</param>
    /// <returns>Lista de procesos pendientes para el usuario</returns>
    [HttpGet("pending-processes")]
    public async Task<ActionResult<List<DocumentaryProcessInstanceDto>>> GetPendingProcesses([FromQuery] Guid? userGuid = null)
    {
        try
        {
            var userId = await GetUserIdFromGuidAsync(userGuid);
            var processes = await _documentaryProcessService.GetPendingProcessesForUserAsync(userId);
            return Ok(processes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene los procesos documentarios disponibles para el área del usuario
    /// </summary>
    /// <param name="userGuid">GUID del usuario opcional</param>
    /// <returns>Lista de procesos disponibles para el área del usuario</returns>
    [HttpGet("available-processes")]
    public async Task<ActionResult<List<DocumentaryProcessInstanceDto>>> GetAvailableProcesses([FromQuery] Guid? userGuid = null)
    {
        try
        {
            var userId = await GetUserIdFromGuidAsync(userGuid);
            var processes = await _documentaryProcessService.GetAvailableProcessesForUserAreaAsync(userId);
            return Ok(processes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene un proceso documentario por su identificador
    /// </summary>
    /// <param name="id">Identificador del proceso documentario</param>
    /// <param name="userGuid">GUID del usuario opcional</param>
    /// <returns>Proceso documentario solicitado</returns>
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult<DocumentaryProcessInstanceDto>> GetByIdAsync(int id, [FromQuery] Guid? userGuid = null)
    {
        try
        {
            var userId = await GetUserIdFromGuidAsync(userGuid);
            var process = await _documentaryProcessService.GetProcessByIdAsync(id, userId);
            
            if (process == null)
                return NotFound("Proceso no encontrado o sin permisos de acceso");

            return Ok(process);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Crea una nueva instancia de proceso documentario
    /// </summary>
    /// <param name="createDto">Datos para crear la instancia del proceso</param>
    /// <returns>Instancia del proceso creada</returns>
    [HttpPost("instance")]
    public async Task<ActionResult<DocumentaryProcessInstanceDto>> CreateProcess([FromBody] CreateDocumentaryProcessInstanceDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var process = await _documentaryProcessService.CreateProcessAsync(createDto, userId);
            return Ok(process);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Toma la responsabilidad de un paso del proceso documentario
    /// </summary>
    /// <param name="stepInstanceId">Identificador de la instancia del paso</param>
    /// <returns>Proceso documentario actualizado</returns>
    [HttpPost("steps/{stepInstanceId}/take")]
    public async Task<ActionResult<DocumentaryProcessInstanceDto>> TakeProcessStep(int stepInstanceId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var process = await _documentaryProcessService.TakeProcessStepAsync(stepInstanceId, userId);
            return Ok(process);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbidden(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado de un paso del proceso documentario
    /// </summary>
    /// <param name="updateDto">Datos para actualizar el paso del proceso</param>
    /// <returns>Proceso documentario actualizado</returns>
    [HttpPut("steps")]
    public async Task<ActionResult<DocumentaryProcessInstanceDto>> UpdateProcessStep([FromBody] UpdateProcessStepDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var process = await _documentaryProcessService.UpdateProcessStepAsync(updateDto, userId);
            return Ok(process);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbidden(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Descarga un documento asociado a un proceso documentario
    /// </summary>
    /// <param name="documentId">Identificador del documento</param>
    /// <returns>Archivo del documento para descarga</returns>
    [HttpGet("documents/{documentId}/download")]
    public async Task<ActionResult> DownloadDocument(int documentId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var fileData = await _documentaryProcessService.DownloadDocumentAsync(documentId, userId);
            
            return File(fileData, "application/octet-stream", $"document_{documentId}");
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbidden(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene las notificaciones de procesos documentarios para el usuario actual
    /// </summary>
    /// <returns>Lista de notificaciones del usuario</returns>
    [HttpGet("notifications")]
    public async Task<ActionResult<List<DocumentaryProcessNotificationDto>>> GetNotifications()
    {
        try
        {
            var userId = GetCurrentUserId();
            var notifications = await _documentaryProcessService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    /// <param name="notificationId">Identificador de la notificación</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("notifications/{notificationId}/mark-read")]
    public async Task<ActionResult> MarkNotificationAsRead(int notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _documentaryProcessService.MarkNotificationAsReadAsync(notificationId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Crea una respuesta HTTP 403 (Forbidden) con un mensaje personalizado
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    /// <returns>Respuesta HTTP 403</returns>
    private ActionResult Forbidden(string message)
    {
        return StatusCode(403, message);
    }
}