using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.DocumentaryProcess;
using SGTD_WebApi.Services;
using System.Security.Claims;

namespace SGTD_WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DocumentaryProcessController : ControllerBase
{
    private readonly IDocumentaryProcessService _documentaryProcessService;

    public DocumentaryProcessController(IDocumentaryProcessService documentaryProcessService)
    {
        _documentaryProcessService = documentaryProcessService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : 0;
    }

    [HttpGet("my-processes")]
    public async Task<ActionResult<List<DocumentaryProcessInstanceDto>>> GetMyProcesses()
    {
        try
        {
            var userId = GetCurrentUserId();
            var processes = await _documentaryProcessService.GetMyProcessesAsync(userId);
            return Ok(processes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("pending-processes")]
    public async Task<ActionResult<List<DocumentaryProcessInstanceDto>>> GetPendingProcesses()
    {
        try
        {
            var userId = GetCurrentUserId();
            var processes = await _documentaryProcessService.GetPendingProcessesForUserAsync(userId);
            return Ok(processes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{processId}")]
    public async Task<ActionResult<DocumentaryProcessInstanceDto>> GetProcessById(int processId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var process = await _documentaryProcessService.GetProcessByIdAsync(processId, userId);
            
            if (process == null)
                return NotFound("Proceso no encontrado o sin permisos de acceso");

            return Ok(process);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
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

    [HttpGet("documents/{documentId}/download")]
    public async Task<ActionResult> DownloadDocument(int documentId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var fileData = await _documentaryProcessService.DownloadDocumentAsync(documentId, userId);
            
            // Obtener información del documento para el nombre del archivo
            // Esto requeriría un método adicional en el servicio, por simplicidad usamos un nombre genérico
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

    private ActionResult Forbidden(string message)
    {
        return StatusCode(403, message);
    }
}