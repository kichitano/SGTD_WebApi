using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Services;
using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class UserFileController : ControllerBase
{
    private readonly IUserFileService _fileService;

    public UserFileController(IUserFileService fileService)
    {
        _fileService = fileService;
    }

    [Route("{userGuid}")]
    [HttpGet]
    public async Task<ActionResult> GetByUserGuIdAsync(Guid userGuid)
    {
        try
        {
            var response = await _fileService.GetByUserGuIdAsync(userGuid);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("upload/{userGuid}")]
    public async Task<ActionResult> UploadFilesAsync([FromForm] List<IFormFile> files, Guid userGuid)
    {
        try
        {
            await _fileService.UploadFilesAsync(files, userGuid);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("download/{id}")]
    public async Task<ActionResult> DownloadFileAsync(int id)
    {
        try
        {
            var downloadedFile = await _fileService.DownloadFileAsync(id);
            return File(downloadedFile.File, "application/octet-stream", downloadedFile.FileName);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("download")]
    public async Task<ActionResult> DownloadMultipleFilesAsync(List<int> ids)
    {
        try
        {
            var zipContent = await _fileService.DownloadMultipleFilesAsync(ids);
            return File(zipContent, "application/zip", "files.zip");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}/{userGuid}")]
    public async Task<ActionResult> DeleteFileAsync(int id, Guid userGuid)
    {
        try
        {
            Console.WriteLine($"DELETE request received - ID: {id}, UserGuid: {userGuid}");
            var result = await _fileService.DeleteFileAsync(id, userGuid);
            Console.WriteLine($"DELETE result: {result}");
            return Ok(new { message = result });
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"ValidationException: {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"UnauthorizedAccessException: {ex.Message}");
            return Forbid(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("delete-multiple/{userGuid}")]
    public async Task<ActionResult> DeleteMultipleFilesAsync([FromBody] List<int> ids, Guid userGuid)
    {
        try
        {
            Console.WriteLine($"DELETE MULTIPLE request received - IDs: [{string.Join(", ", ids)}], UserGuid: {userGuid}");
            var result = await _fileService.DeleteMultipleFilesAsync(ids, userGuid);
            Console.WriteLine($"DELETE MULTIPLE result: {result}");
            return Ok(new { message = result });
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"ValidationException (Multiple): {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"UnauthorizedAccessException (Multiple): {ex.Message}");
            return Forbid(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception (Multiple): {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("share-info/{fileId}/{userGuid}")]
    public async Task<ActionResult> GetFileShareInfoAsync(int fileId, Guid userGuid)
    {
        try
        {
            var result = await _fileService.GetFileShareInfoAsync(fileId, userGuid);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("share/{fileId}/{userGuid}")]
    public async Task<ActionResult> ShareFileAsync(int fileId, [FromBody] List<int> userIds, Guid userGuid)
    {
        try
        {
            var result = await _fileService.ShareFileAsync(fileId, userIds, userGuid);
            return Ok(new { message = result });
        }
        catch (ValidationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("unshare/{fileId}/{userId}/{userGuid}")]
    public async Task<ActionResult> UnshareFileAsync(int fileId, int userId, Guid userGuid)
    {
        try
        {
            var result = await _fileService.UnshareFileAsync(fileId, userId, userGuid);
            return Ok(new { message = result });
        }
        catch (ValidationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("shared/{userGuid}")]
    public async Task<ActionResult> GetSharedFilesAsync(Guid userGuid)
    {
        try
        {
            var result = await _fileService.GetSharedFilesAsync(userGuid);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}