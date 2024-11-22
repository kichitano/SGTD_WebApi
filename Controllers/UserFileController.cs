using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IUserFileService _fileService;

        public FileController(IUserFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
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
    }
}