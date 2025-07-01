using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.DocumentaryProcess;

namespace SGTD_WebApi.Services.Implementation;

public class DocumentaryProcessService : IDocumentaryProcessService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public DocumentaryProcessService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Standard CRUD methods following Area/Position pattern
    public async Task CreateAsync(DocumentaryProcessRequestParams requestParams, int userId)
    {
        var existingProcess = await _context.DocumentaryProcessInstances
            .FirstOrDefaultAsync(p => p.ProcessNumber == requestParams.Name);

        if (existingProcess != null)
            throw new ArgumentException("Ya existe un proceso con este nombre");

        var entity = new DocumentaryProcessInstance
        {
            ProcessNumber = requestParams.Name,
            DocumentaryProcedureId = requestParams.DocumentaryProcessId,
            RequestedByUserId = userId,
            CurrentStepOrder = 1,
            Status = requestParams.Status ? DocumentaryProcessStatus.InProgress : DocumentaryProcessStatus.Cancelled,
            Notes = requestParams.Description,
            RequestedAt = DateTime.UtcNow
        };

        _context.DocumentaryProcessInstances.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DocumentaryProcessRequestParams requestParams, int userId)
    {
        if (!requestParams.Id.HasValue)
            throw new ArgumentNullException(nameof(requestParams.Id));

        var entity = await _context.DocumentaryProcessInstances
            .FirstOrDefaultAsync(p => p.Id == requestParams.Id.Value);

        if (entity == null)
            throw new ArgumentException("Proceso no encontrado");

        if (entity.RequestedByUserId != userId)
            throw new UnauthorizedAccessException("No tiene permisos para editar este proceso");

        entity.ProcessNumber = requestParams.Name;
        entity.Notes = requestParams.Description;
        entity.Status = requestParams.Status ? DocumentaryProcessStatus.InProgress : DocumentaryProcessStatus.Cancelled;

        await _context.SaveChangesAsync();
    }

    public async Task<List<DocumentaryProcessInstanceDto>> GetAllAsync(int userId)
    {
        var processes = await _context.DocumentaryProcessInstances
            .Include(p => p.DocumentaryProcedure)
            .Include(p => p.RequestedByUser)
                .ThenInclude(u => u.Person)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Area)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Position)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.AssignedToUser)
                    .ThenInclude(u => u.Person)
            .Include(p => p.Documents)
                .ThenInclude(d => d.DocumentType)
            .OrderByDescending(p => p.RequestedAt)
            .ToListAsync();

        // Filter based on user permissions
        var user = await _context.Users.Include(u => u.Position).FirstOrDefaultAsync(u => u.Id == userId);
        var filteredProcesses = processes.Where(p => 
            p.RequestedByUserId == userId || 
            p.StepInstances.Any(si => si.DocumentaryProcedureStep.PositionId == user?.PositionId)
        ).ToList();

        return filteredProcesses.Select(p => MapToDto(p, userId)).ToList();
    }

    public async Task DeleteByIdAsync(int id, int userId)
    {
        var entity = await _context.DocumentaryProcessInstances
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity == null)
            throw new ArgumentException("Proceso no encontrado");

        if (entity.RequestedByUserId != userId)
            throw new UnauthorizedAccessException("No tiene permisos para eliminar este proceso");

        if (entity.Status == DocumentaryProcessStatus.InProgress)
            throw new InvalidOperationException("No se puede eliminar un proceso en progreso");

        _context.DocumentaryProcessInstances.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<DocumentaryProcessInstanceDto>> GetMyProcessesAsync(int userId)
    {
        var processes = await _context.DocumentaryProcessInstances
            .Include(p => p.DocumentaryProcedure)
            .Include(p => p.RequestedByUser)
                .ThenInclude(u => u.Person)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Area)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Position)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.AssignedToUser)
                    .ThenInclude(u => u.Person)
            .Include(p => p.Documents)
                .ThenInclude(d => d.DocumentType)
            .Where(p => p.RequestedByUserId == userId)
            .OrderByDescending(p => p.RequestedAt)
            .ToListAsync();

        return processes.Select(p => MapToDto(p)).ToList();
    }

    public async Task<List<DocumentaryProcessInstanceDto>> GetPendingProcessesForUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Position)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.PositionId == null)
            return new List<DocumentaryProcessInstanceDto>();

        var pendingSteps = await _context.DocumentaryProcessStepInstances
            .Include(si => si.DocumentaryProcessInstance)
                .ThenInclude(pi => pi.DocumentaryProcedure)
            .Include(si => si.DocumentaryProcessInstance)
                .ThenInclude(pi => pi.RequestedByUser)
                    .ThenInclude(u => u.Person)
            .Include(si => si.DocumentaryProcedureStep)
                .ThenInclude(ps => ps.Area)
            .Include(si => si.DocumentaryProcedureStep)
                .ThenInclude(ps => ps.Position)
            .Include(si => si.AssignedToUser)
                .ThenInclude(u => u.Person)
            .Include(si => si.Documents)
                .ThenInclude(d => d.DocumentType)
            .Where(si => si.DocumentaryProcedureStep.PositionId == user.PositionId && 
                        si.Status == DocumentaryStepStatus.Pending &&
                        si.DocumentaryProcessInstance.Status == DocumentaryProcessStatus.InProgress)
            .ToListAsync();

        var processIds = pendingSteps.Select(si => si.DocumentaryProcessInstanceId).Distinct();

        var processes = await _context.DocumentaryProcessInstances
            .Include(p => p.DocumentaryProcedure)
            .Include(p => p.RequestedByUser)
                .ThenInclude(u => u.Person)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Area)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Position)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.AssignedToUser)
                    .ThenInclude(u => u.Person)
            .Include(p => p.Documents)
                .ThenInclude(d => d.DocumentType)
            .Where(p => processIds.Contains(p.Id))
            .ToListAsync();

        return processes.Select(p => MapToDto(p, userId)).ToList();
    }

    public async Task<List<DocumentaryProcessInstanceDto>> GetAvailableProcessesForUserAreaAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Position)
                .ThenInclude(p => p.Area)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Position?.AreaId == null)
            return new List<DocumentaryProcessInstanceDto>();

        // Get processes that have unassigned steps in the user's area
        var availableSteps = await _context.DocumentaryProcessStepInstances
            .Include(si => si.DocumentaryProcessInstance)
                .ThenInclude(pi => pi.DocumentaryProcedure)
            .Include(si => si.DocumentaryProcessInstance)
                .ThenInclude(pi => pi.RequestedByUser)
                    .ThenInclude(u => u.Person)
            .Include(si => si.DocumentaryProcedureStep)
                .ThenInclude(ps => ps.Area)
            .Include(si => si.DocumentaryProcedureStep)
                .ThenInclude(ps => ps.Position)
            .Where(si => si.DocumentaryProcedureStep.AreaId == user.Position.AreaId && 
                        si.Status == DocumentaryStepStatus.Pending &&
                        si.AssignedToUserId == null &&
                        si.DocumentaryProcessInstance.Status == DocumentaryProcessStatus.InProgress)
            .ToListAsync();

        var processIds = availableSteps.Select(si => si.DocumentaryProcessInstanceId).Distinct();

        var processes = await _context.DocumentaryProcessInstances
            .Include(p => p.DocumentaryProcedure)
            .Include(p => p.RequestedByUser)
                .ThenInclude(u => u.Person)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Area)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Position)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.AssignedToUser)
                    .ThenInclude(u => u.Person)
            .Include(p => p.Documents)
                .ThenInclude(d => d.DocumentType)
            .Where(p => processIds.Contains(p.Id))
            .ToListAsync();

        return processes.Select(p => MapToDto(p)).ToList();
    }

    public async Task<DocumentaryProcessInstanceDto?> GetProcessByIdAsync(int processId, int userId)
    {
        var process = await _context.DocumentaryProcessInstances
            .Include(p => p.DocumentaryProcedure)
            .Include(p => p.RequestedByUser)
                .ThenInclude(u => u.Person)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Area)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
                    .ThenInclude(ps => ps.Position)
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.AssignedToUser)
                    .ThenInclude(u => u.Person)
            .Include(p => p.Documents)
                .ThenInclude(d => d.DocumentType)
            .FirstOrDefaultAsync(p => p.Id == processId);

        if (process == null) return null;

        // Verificar que el usuario tenga acceso al proceso
        var user = await _context.Users.Include(u => u.Position).FirstOrDefaultAsync(u => u.Id == userId);
        var hasAccess = process.RequestedByUserId == userId || 
                       process.StepInstances.Any(si => si.DocumentaryProcedureStep.PositionId == user?.PositionId);

        return hasAccess ? MapToDto(process, userId) : null;
    }

    public async Task<DocumentaryProcessInstanceDto> CreateProcessAsync(CreateDocumentaryProcessInstanceDto createDto, int userId)
    {
        var procedure = await _context.DocumentaryProcedures
            .Include(p => p.Area)
            .FirstOrDefaultAsync(p => p.Id == createDto.DocumentaryProcedureId);

        if (procedure == null)
            throw new ArgumentException("Procedimiento documentario no encontrado");

        var steps = await _context.DocumentaryProcedureSteps
            .Include(s => s.Area)
            .Include(s => s.Position)
            .Where(s => s.DocumentaryProcedureId == createDto.DocumentaryProcedureId)
            .OrderBy(s => s.Order)
            .ToListAsync();

        if (!steps.Any())
            throw new ArgumentException("El procedimiento no tiene pasos configurados");

        // Generar número de proceso único
        var processNumber = await GenerateProcessNumberAsync();

        var processInstance = new DocumentaryProcessInstance
        {
            ProcessNumber = processNumber,
            DocumentaryProcedureId = createDto.DocumentaryProcedureId,
            RequestedByUserId = userId,
            CurrentStepOrder = 1,
            Status = DocumentaryProcessStatus.InProgress,
            Notes = createDto.Notes,
            RequestedAt = DateTime.UtcNow
        };

        _context.DocumentaryProcessInstances.Add(processInstance);
        await _context.SaveChangesAsync();

        // Crear instancias de pasos
        foreach (var step in steps)
        {
            var stepInstance = new DocumentaryProcessStepInstance
            {
                DocumentaryProcessInstanceId = processInstance.Id,
                DocumentaryProcedureStepId = step.Id,
                Status = step.Order == 1 ? DocumentaryStepStatus.Pending : DocumentaryStepStatus.Pending
            };

            _context.DocumentaryProcessStepInstances.Add(stepInstance);
        }

        // Guardar documentos upload iniciales
        await SaveUploadDocumentsAsync(processInstance.Id, null, createDto.UploadDocuments, userId);

        await _context.SaveChangesAsync();

        // Enviar notificaciones para el primer paso
        await NotifyStepAssignmentAsync(processInstance.Id, 1);

        return await GetProcessByIdAsync(processInstance.Id, userId);
    }

    public async Task<DocumentaryProcessInstanceDto> TakeProcessStepAsync(int processStepInstanceId, int userId)
    {
        var stepInstance = await _context.DocumentaryProcessStepInstances
            .Include(si => si.DocumentaryProcessInstance)
            .Include(si => si.DocumentaryProcedureStep)
            .FirstOrDefaultAsync(si => si.Id == processStepInstanceId);

        if (stepInstance == null)
            throw new ArgumentException("Paso del proceso no encontrado");

        var user = await _context.Users.Include(u => u.Position).FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user?.PositionId != stepInstance.DocumentaryProcedureStep.PositionId)
            throw new UnauthorizedAccessException("No tiene permisos para tomar este paso");

        if (stepInstance.Status != DocumentaryStepStatus.Pending)
            throw new InvalidOperationException("El paso ya ha sido tomado o completado");

        stepInstance.AssignedToUserId = userId;
        stepInstance.Status = DocumentaryStepStatus.InProgress;
        stepInstance.StartedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetProcessByIdAsync(stepInstance.DocumentaryProcessInstanceId, userId);
    }

    public async Task<DocumentaryProcessInstanceDto> UpdateProcessStepAsync(UpdateProcessStepDto updateDto, int userId)
    {
        var stepInstance = await _context.DocumentaryProcessStepInstances
            .Include(si => si.DocumentaryProcessInstance)
            .Include(si => si.DocumentaryProcedureStep)
            .FirstOrDefaultAsync(si => si.Id == updateDto.ProcessStepInstanceId);

        if (stepInstance == null)
            throw new ArgumentException("Paso del proceso no encontrado");

        if (stepInstance.AssignedToUserId != userId)
            throw new UnauthorizedAccessException("No tiene permisos para actualizar este paso");

        stepInstance.Status = updateDto.Status;
        stepInstance.Notes = updateDto.Notes;

        if (updateDto.Status == DocumentaryStepStatus.Completed)
        {
            stepInstance.CompletedAt = DateTime.UtcNow;

            // Guardar documentos del paso
            await SaveUploadDocumentsAsync(stepInstance.DocumentaryProcessInstanceId, stepInstance.Id, updateDto.Documents, userId);

            // Avanzar al siguiente paso o completar proceso
            await AdvanceProcessAsync(stepInstance.DocumentaryProcessInstanceId);
        }
        else if (updateDto.Status == DocumentaryStepStatus.Rejected)
        {
            // Rechazar todo el proceso
            stepInstance.DocumentaryProcessInstance.Status = DocumentaryProcessStatus.Rejected;
            stepInstance.DocumentaryProcessInstance.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return await GetProcessByIdAsync(stepInstance.DocumentaryProcessInstanceId, userId);
    }

    public async Task<byte[]> DownloadDocumentAsync(int documentId, int userId)
    {
        var document = await _context.DocumentaryProcessDocuments
            .Include(d => d.DocumentaryProcessInstance)
            .Include(d => d.DocumentaryProcessStepInstance)
                .ThenInclude(si => si.DocumentaryProcedureStep)
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null)
            throw new ArgumentException("Documento no encontrado");

        var user = await _context.Users.Include(u => u.Position).FirstOrDefaultAsync(u => u.Id == userId);
        
        // Verificar permisos de descarga
        var canDownload = document.DocumentaryProcessInstance.RequestedByUserId == userId ||
                         document.DocumentaryProcessStepInstance?.DocumentaryProcedureStep.PositionId == user?.PositionId;

        if (!canDownload)
            throw new UnauthorizedAccessException("No tiene permisos para descargar este documento");

        // Verificar si es documento del último paso para usuario solicitante
        if (document.DocumentaryProcessInstance.RequestedByUserId == userId && document.DocumentType.IsUploadable)
        {
            var maxStepOrder = await _context.DocumentaryProcessStepInstances
                .Where(si => si.DocumentaryProcessInstanceId == document.DocumentaryProcessInstanceId)
                .MaxAsync(si => si.DocumentaryProcedureStep.Order);

            var isLastStep = document.DocumentaryProcessStepInstance?.DocumentaryProcedureStep.Order == maxStepOrder;
            if (!isLastStep)
                throw new UnauthorizedAccessException("Solo puede descargar documentos del último paso completado");
        }

        return await File.ReadAllBytesAsync(document.FilePath);
    }

    public async Task<List<DocumentaryProcessNotificationDto>> GetUserNotificationsAsync(int userId)
    {
        var notifications = await _context.DocumentaryProcessNotifications
            .Include(n => n.DocumentaryProcessInstance)
                .ThenInclude(pi => pi.DocumentaryProcedure)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();

        return notifications.Select(n => new DocumentaryProcessNotificationDto
        {
            Id = n.Id,
            DocumentaryProcessInstanceId = n.DocumentaryProcessInstanceId,
            ProcessNumber = n.DocumentaryProcessInstance.ProcessNumber,
            DocumentaryProcedureName = n.DocumentaryProcessInstance.DocumentaryProcedure.Name,
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            TypeName = GetNotificationTypeName(n.Type),
            IsRead = n.IsRead,
            SentAt = n.SentAt,
            ReadAt = n.ReadAt
        }).ToList();
    }

    public async Task MarkNotificationAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.DocumentaryProcessNotifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    #region Private Methods

    private async Task<string> GenerateProcessNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var lastProcess = await _context.DocumentaryProcessInstances
            .Where(p => p.ProcessNumber.StartsWith(year.ToString()))
            .OrderByDescending(p => p.ProcessNumber)
            .FirstOrDefaultAsync();

        var sequence = 1;
        if (lastProcess != null)
        {
            var lastNumber = lastProcess.ProcessNumber.Split('-').LastOrDefault();
            if (int.TryParse(lastNumber, out int last))
                sequence = last + 1;
        }

        return $"{year}-{sequence:D6}";
    }

    private async Task SaveUploadDocumentsAsync(int processInstanceId, int? stepInstanceId, List<UploadDocumentDto> documents, int userId)
    {
        var folderPath = Path.Combine(_configuration["FileSettings:ProcessDocumentsPath"] ?? "Storage/ProcessDocuments", processInstanceId.ToString());
        Directory.CreateDirectory(folderPath);

        foreach (var doc in documents)
        {
            var fileName = $"{Guid.NewGuid()}_{doc.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            var fileBytes = Convert.FromBase64String(doc.FileData);
            await File.WriteAllBytesAsync(filePath, fileBytes);

            var processDocument = new DocumentaryProcessDocument
            {
                DocumentaryProcessInstanceId = processInstanceId,
                DocumentaryProcessStepInstanceId = stepInstanceId,
                DocumentTypeId = doc.DocumentTypeId,
                UploadedByUserId = userId,
                FileName = doc.FileName,
                FilePath = filePath,
                ContentType = doc.ContentType,
                FileSize = fileBytes.Length,
                IsSignatureRequired = doc.IsSignatureRequired,
                UploadedAt = DateTime.UtcNow
            };

            _context.DocumentaryProcessDocuments.Add(processDocument);
        }
    }

    private async Task AdvanceProcessAsync(int processInstanceId)
    {
        var process = await _context.DocumentaryProcessInstances
            .Include(p => p.StepInstances)
                .ThenInclude(si => si.DocumentaryProcedureStep)
            .FirstOrDefaultAsync(p => p.Id == processInstanceId);

        var maxOrder = process.StepInstances.Max(si => si.DocumentaryProcedureStep.Order);
        
        if (process.CurrentStepOrder >= maxOrder)
        {
            // Proceso completado
            process.Status = DocumentaryProcessStatus.Completed;
            process.CompletedAt = DateTime.UtcNow;
            
            await NotifyProcessCompletedAsync(processInstanceId);
        }
        else
        {
            // Avanzar al siguiente paso
            process.CurrentStepOrder++;
            await NotifyStepAssignmentAsync(processInstanceId, process.CurrentStepOrder);
        }
    }

    private async Task NotifyStepAssignmentAsync(int processInstanceId, int stepOrder)
    {
        var processInstance = await _context.DocumentaryProcessInstances
            .Include(p => p.DocumentaryProcedure)
            .Include(p => p.RequestedByUser)
                .ThenInclude(u => u.Person)
            .FirstOrDefaultAsync(p => p.Id == processInstanceId);

        var step = await _context.DocumentaryProcedureSteps
            .Include(s => s.Position)
            .Include(s => s.Area)
            .FirstOrDefaultAsync(s => s.DocumentaryProcedureId == processInstance.DocumentaryProcedureId && s.Order == stepOrder);

        var usersInPosition = await _context.Users
            .Include(u => u.Person)
            .Where(u => u.PositionId == step.PositionId)
            .ToListAsync();

        foreach (var user in usersInPosition)
        {
            var notification = new DocumentaryProcessNotification
            {
                DocumentaryProcessInstanceId = processInstanceId,
                UserId = user.Id,
                Title = "Nuevo trámite asignado",
                Message = $"Se le ha asignado el trámite {processInstance.ProcessNumber} del procedimiento {processInstance.DocumentaryProcedure.Name}",
                Type = DocumentaryNotificationType.StepAssigned,
                SentAt = DateTime.UtcNow
            };

            _context.DocumentaryProcessNotifications.Add(notification);
        }
    }

    private async Task NotifyProcessCompletedAsync(int processInstanceId)
    {
        var processInstance = await _context.DocumentaryProcessInstances
            .Include(p => p.DocumentaryProcedure)
            .Include(p => p.RequestedByUser)
            .FirstOrDefaultAsync(p => p.Id == processInstanceId);

        var notification = new DocumentaryProcessNotification
        {
            DocumentaryProcessInstanceId = processInstanceId,
            UserId = processInstance.RequestedByUserId,
            Title = "Trámite completado",
            Message = $"Su trámite {processInstance.ProcessNumber} del procedimiento {processInstance.DocumentaryProcedure.Name} ha sido completado",
            Type = DocumentaryNotificationType.ProcessCompleted,
            SentAt = DateTime.UtcNow
        };

        _context.DocumentaryProcessNotifications.Add(notification);
    }

    private DocumentaryProcessInstanceDto MapToDto(DocumentaryProcessInstance process, int? currentUserId = null)
    {
        var dto = new DocumentaryProcessInstanceDto
        {
            Id = process.Id,
            ProcessNumber = process.ProcessNumber,
            DocumentaryProcedureId = process.DocumentaryProcedureId,
            DocumentaryProcedureName = process.DocumentaryProcedure.Name,
            RequestedByUserId = process.RequestedByUserId,
            RequestedByUserName = $"{process.RequestedByUser.Person.FirstName} {process.RequestedByUser.Person.LastName}",
            RequestedByUserEmail = process.RequestedByUser.Email,
            CurrentStepOrder = process.CurrentStepOrder,
            Status = process.Status,
            StatusName = GetStatusName(process.Status),
            Notes = process.Notes,
            RequestedAt = process.RequestedAt,
            CompletedAt = process.CompletedAt,
            StepInstances = process.StepInstances.OrderBy(si => si.DocumentaryProcedureStep.Order).Select(si => MapStepToDto(si, currentUserId)).ToList(),
            Documents = process.Documents.Select(MapDocumentToDto).ToList()
        };

        // Separar documentos por tipo
        dto.RequiredUploadDocuments = dto.Documents.Where(d => d.DocumentTypeIsUploadable && d.StepOrder == null).ToList();
        dto.AvailableDownloadDocuments = dto.Documents.Where(d => !d.DocumentTypeIsUploadable && CanUserDownloadDocument(d, currentUserId, process)).ToList();

        return dto;
    }

    private DocumentaryProcessStepInstanceDto MapStepToDto(DocumentaryProcessStepInstance stepInstance, int? currentUserId)
    {
        return new DocumentaryProcessStepInstanceDto
        {
            Id = stepInstance.Id,
            DocumentaryProcessInstanceId = stepInstance.DocumentaryProcessInstanceId,
            DocumentaryProcedureStepId = stepInstance.DocumentaryProcedureStepId,
            StepOrder = stepInstance.DocumentaryProcedureStep.Order,
            AreaName = stepInstance.DocumentaryProcedureStep.Area.Name,
            PositionName = stepInstance.DocumentaryProcedureStep.Position.Name,
            AssignedToUserId = stepInstance.AssignedToUserId,
            AssignedToUserName = stepInstance.AssignedToUser != null ? 
                $"{stepInstance.AssignedToUser.Person.FirstName} {stepInstance.AssignedToUser.Person.LastName}" : null,
            AssignedToUserEmail = stepInstance.AssignedToUser?.Email,
            Status = stepInstance.Status,
            StatusName = GetStepStatusName(stepInstance.Status),
            Notes = stepInstance.Notes,
            StartedAt = stepInstance.StartedAt,
            CompletedAt = stepInstance.CompletedAt,
            Documents = stepInstance.Documents.Select(MapDocumentToDto).ToList(),
            CanTakeAction = currentUserId.HasValue && stepInstance.AssignedToUserId == currentUserId.Value,
            IsCurrentStep = stepInstance.DocumentaryProcessInstance.CurrentStepOrder == stepInstance.DocumentaryProcedureStep.Order
        };
    }

    private DocumentaryProcessDocumentDto MapDocumentToDto(DocumentaryProcessDocument document)
    {
        return new DocumentaryProcessDocumentDto
        {
            Id = document.Id,
            DocumentaryProcessInstanceId = document.DocumentaryProcessInstanceId,
            DocumentaryProcessStepInstanceId = document.DocumentaryProcessStepInstanceId,
            DocumentTypeId = document.DocumentTypeId,
            DocumentTypeName = document.DocumentType.Name,
            DocumentTypeIsUploadable = document.DocumentType.IsUploadable,
            UploadedByUserId = document.UploadedByUserId,
            UploadedByUserName = $"{document.UploadedByUser.Person.FirstName} {document.UploadedByUser.Person.LastName}",
            FileName = document.FileName,
            FilePath = document.FilePath,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            UploadedAt = document.UploadedAt,
            IsSignatureRequired = document.IsSignatureRequired,
            IsSigned = document.IsSigned,
            StepOrder = document.DocumentaryProcessStepInstance?.DocumentaryProcedureStep.Order
        };
    }

    private bool CanUserDownloadDocument(DocumentaryProcessDocumentDto document, int? userId, DocumentaryProcessInstance process)
    {
        if (!userId.HasValue) return false;

        // El solicitante solo puede descargar documentos del último paso
        if (process.RequestedByUserId == userId.Value)
        {
            var maxCompletedStep = process.StepInstances
                .Where(si => si.Status == DocumentaryStepStatus.Completed)
                .Max(si => si.DocumentaryProcedureStep.Order);
            
            return document.StepOrder == maxCompletedStep;
        }

        // Los usuarios del paso pueden descargar documentos de su paso
        return true;
    }

    private string GetStatusName(DocumentaryProcessStatus status)
    {
        return status switch
        {
            DocumentaryProcessStatus.InProgress => "En Proceso",
            DocumentaryProcessStatus.Completed => "Completado",
            DocumentaryProcessStatus.Rejected => "Rechazado",
            DocumentaryProcessStatus.Cancelled => "Cancelado",
            _ => "Desconocido"
        };
    }

    private string GetStepStatusName(DocumentaryStepStatus status)
    {
        return status switch
        {
            DocumentaryStepStatus.Pending => "Pendiente",
            DocumentaryStepStatus.InProgress => "En Proceso",
            DocumentaryStepStatus.Completed => "Completado",
            DocumentaryStepStatus.Rejected => "Rechazado",
            _ => "Desconocido"
        };
    }

    private string GetNotificationTypeName(DocumentaryNotificationType type)
    {
        return type switch
        {
            DocumentaryNotificationType.ProcessStarted => "Proceso Iniciado",
            DocumentaryNotificationType.StepAssigned => "Paso Asignado",
            DocumentaryNotificationType.StepCompleted => "Paso Completado",
            DocumentaryNotificationType.ProcessCompleted => "Proceso Completado",
            DocumentaryNotificationType.ProcessRejected => "Proceso Rechazado",
            DocumentaryNotificationType.DocumentRequired => "Documento Requerido",
            _ => "Desconocido"
        };
    }

    #endregion
}