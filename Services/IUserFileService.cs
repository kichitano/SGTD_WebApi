using SGTD_WebApi.Models.UserFile;

namespace SGTD_WebApi.Services;

public interface IUserFileService
{
    Task<List<UserFileDto>> GetByUserGuIdAsync(Guid userGuid);
    Task UploadFilesAsync(List<IFormFile> userFiles, Guid userGuid);
    Task<UserFileByteDto> DownloadFileAsync(int id);
    Task<byte[]> DownloadMultipleFilesAsync(List<int> ids);
}