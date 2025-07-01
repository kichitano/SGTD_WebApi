using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.DocumentaryProcess;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class DocumentaryProcessController : Controller
{
    private readonly IDocumentaryProcessService _documentaryProcessService;
    private readonly IUserService _userService;

    public DocumentaryProcessController(IDocumentaryProcessService documentaryProcessService, IUserService userService)
    {
        _documentaryProcessService = documentaryProcessService;
        _userService = userService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : 0;
    }

    private async Task<int> GetUserIdFromGuidAsync(Guid? userGuid)
    {
        if (userGuid.HasValue)
        {
            var user = await _userService.GetByGuidAsync(userGuid.Value);
            return user?.Id ?? 0;
        }
        return GetCurrentUserId();
    }

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