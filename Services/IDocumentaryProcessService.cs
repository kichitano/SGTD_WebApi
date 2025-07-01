using SGTD_WebApi.Models.DocumentaryProcess;

namespace SGTD_WebApi.Services;

public interface IDocumentaryProcessService
{
    Task CreateAsync(DocumentaryProcessRequestParams requestParams, int userId);
    Task UpdateAsync(DocumentaryProcessRequestParams requestParams, int userId);
    Task<List<DocumentaryProcessInstanceDto>> GetAllAsync(int userId);
    Task DeleteByIdAsync(int id, int userId);

    Task<List<DocumentaryProcessInstanceDto>> GetMyProcessesAsync(int userId);
    Task<List<DocumentaryProcessInstanceDto>> GetPendingProcessesForUserAsync(int userId);
    Task<List<DocumentaryProcessInstanceDto>> GetAvailableProcessesForUserAreaAsync(int userId);
    Task<DocumentaryProcessInstanceDto?> GetProcessByIdAsync(int processId, int userId);
    Task<DocumentaryProcessInstanceDto> CreateProcessAsync(CreateDocumentaryProcessInstanceDto createDto, int userId);
    Task<DocumentaryProcessInstanceDto> TakeProcessStepAsync(int processStepInstanceId, int userId);
    Task<DocumentaryProcessInstanceDto> UpdateProcessStepAsync(UpdateProcessStepDto updateDto, int userId);
    Task<byte[]> DownloadDocumentAsync(int documentId, int userId);
    Task<List<DocumentaryProcessNotificationDto>> GetUserNotificationsAsync(int userId);
    Task MarkNotificationAsReadAsync(int notificationId, int userId);
}