using SGTD_WebApi.Models.UserFile;

namespace SGTD_WebApi.Services;

public interface IUserFileService
{
    Task<List<UserFileDto>> GetByUserGuIdAsync(Guid userGuid);
    Task UploadFilesAsync(List<IFormFile> userFiles, Guid userGuid);
    Task<UserFileByteDto> DownloadFileAsync(int id);
    Task<byte[]> DownloadMultipleFilesAsync(List<int> ids);
    Task<string> DeleteFileAsync(int id, Guid userGuid);
    Task<string> DeleteMultipleFilesAsync(List<int> ids, Guid userGuid);
    Task<FileShareInfoDto> GetFileShareInfoAsync(int fileId, Guid userGuid);
    Task<string> ShareFileAsync(int fileId, List<int> personIds, Guid sharedByUserGuid);
    Task<string> UnshareFileAsync(int fileId, int userId, Guid userGuid);
    Task<List<UserFileShareDto>> GetSharedFilesAsync(Guid userGuid);
}