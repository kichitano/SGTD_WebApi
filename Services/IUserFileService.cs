using System.IO;
using SGTD_WebApi.Models.UserFile;

namespace SGTD_WebApi.Services;

public interface IUserFileService
{
    Task UploadFilesAsync(List<IFormFile> userFiles, Guid userGuid);
    Task<UserFileDto> DownloadFileAsync(int id);
    Task<byte[]> DownloadMultipleFilesAsync(List<int> ids);
}