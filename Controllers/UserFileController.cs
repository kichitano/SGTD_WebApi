using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Services;
using System.ComponentModel.DataAnnotations;
using SGTD_WebApi.Models.UserFile;

namespace SGTD_WebApi.Controllers;

/// <summary>
/// Controlador para gestionar archivos de usuarios del sistema
/// </summary>
[Route("[controller]")]
[ApiController]
public class UserFileController : ControllerBase
{
    private readonly IUserFileService _fileService;

    /// <summary>
    /// Constructor del controlador de archivos de usuario
    /// </summary>
    /// <param name="fileService">Servicio para gestionar archivos de usuario</param>
    public UserFileController(IUserFileService fileService)
    {
        _fileService = fileService;
    }

    /// <summary>
    /// Obtiene todos los archivos asociados a un usuario específico
    /// </summary>
    /// <param name="userGuid">GUID del usuario para buscar archivos</param>
    /// <returns>Lista de archivos del usuario especificado</returns>
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

    /// <summary>
    /// Sube múltiples archivos para un usuario específico
    /// </summary>
    /// <param name="files">Lista de archivos a subir</param>
    /// <param name="userGuid">GUID del usuario propietario de los archivos</param>
    /// <returns>Resultado de la operación de subida</returns>
    [HttpPost("upload/{userGuid}")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
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

    /// <summary>
    /// Descarga un archivo específico mediante su ID
    /// </summary>
    /// <param name="id">ID del archivo a descargar</param>
    /// <returns>Archivo como stream para descarga</returns>
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

    /// <summary>
    /// Descarga múltiples archivos comprimidos en un archivo ZIP
    /// </summary>
    /// <param name="ids">Lista de IDs de archivos a descargar</param>
    /// <returns>Archivo ZIP con los archivos solicitados</returns>
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

    /// <summary>
    /// Elimina un archivo específico del usuario
    /// </summary>
    /// <param name="request">Datos de la solicitud con ID del archivo y GUID del usuario</param>
    /// <returns>Resultado de la operación de eliminación</returns>
    [HttpPost("delete")]
    public async Task<ActionResult> DeleteFileAsync([FromBody] DeleteFileRequestDto request)
    {
        try
        {
            Console.WriteLine($"DELETE request received - ID: {request.FileId}, UserGuid: {request.UserGuid}");
            var result = await _fileService.DeleteFileAsync(request.FileId, request.UserGuid);
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
            return StatusCode(403, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina múltiples archivos del usuario en una sola operación
    /// </summary>
    /// <param name="request">Datos de la solicitud con IDs de archivos y GUID del usuario</param>
    /// <returns>Resultado de la operación de eliminación múltiple</returns>
    [HttpPost("delete-multiple")]
    public async Task<ActionResult> DeleteMultipleFilesAsync([FromBody] DeleteMultipleFilesRequestDto request)
    {
        try
        {
            Console.WriteLine($"DELETE MULTIPLE request received - IDs: [{string.Join(", ", request.FileIds)}], UserGuid: {request.UserGuid}");
            var result = await _fileService.DeleteMultipleFilesAsync(request.FileIds, request.UserGuid);
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
            return StatusCode(403, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception (Multiple): {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene información de compartición de un archivo específico
    /// </summary>
    /// <param name="fileId">ID del archivo</param>
    /// <param name="userGuid">GUID del usuario propietario</param>
    /// <returns>Información de compartición del archivo</returns>
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

    /// <summary>
    /// Comparte un archivo con personas específicas
    /// </summary>
    /// <param name="fileId">ID del archivo a compartir</param>
    /// <param name="personIds">Lista de IDs de personas con las que compartir</param>
    /// <param name="userGuid">GUID del usuario propietario del archivo</param>
    /// <returns>Resultado de la operación de compartición</returns>
    [HttpPost("share/{fileId}/{userGuid}")]
    public async Task<ActionResult> ShareFileAsync(int fileId, [FromBody] List<int> personIds, Guid userGuid)
    {
        try
        {
            var result = await _fileService.ShareFileAsync(fileId, personIds, userGuid);
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

    /// <summary>
    /// Deja de compartir un archivo con un usuario específico
    /// </summary>
    /// <param name="requestParams">Parámetros con información del archivo y usuario</param>
    /// <returns>Resultado de la operación de descompartir</returns>
    [Route("unshare")]
    [HttpPost]
    public async Task<ActionResult> UnshareFileAsync([FromBody] UnshareFileRequestParams requestParams)
    {
        try
        {
            var result = await _fileService.UnshareFileAsync(requestParams.FileId, requestParams.UserId, requestParams.UserGuid);
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

    /// <summary>
    /// Obtiene todos los archivos compartidos con un usuario específico
    /// </summary>
    /// <param name="userGuid">GUID del usuario para buscar archivos compartidos</param>
    /// <returns>Lista de archivos compartidos con el usuario</returns>
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