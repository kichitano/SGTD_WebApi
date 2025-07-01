using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.DocumentaryProcedure;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar procedimientos documentarios.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar procedimientos documentarios,
/// incluyendo la gestión de pasos y documentos asociados a cada procedimiento.
/// </summary>
public class DocumentaryProcedureService : IDocumentaryProcedureService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de procedimientos documentarios.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public DocumentaryProcedureService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un nuevo procedimiento documentario de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del procedimiento a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe un trámite documentario con el mismo nombre.</exception>
    public async Task CreateAsync(DocumentaryProcedureRequestParams requestParams)
    {
        if (await IsProcedureNameUniqueAsync(requestParams.Name))
        {
            var procedure = new DocumentaryProcedure
            {
                Name = requestParams.Name,
                Description = requestParams.Description,
                Status = requestParams.Status,
                AreaId = requestParams.AreaId
            };

            _context.DocumentaryProcedures.Add(procedure);
            await _context.SaveChangesAsync();

            await CreateStepsAsync(procedure.Id, requestParams.Steps);
        }
        else
        {
            throw new InvalidOperationException("Ya existe un trámite documentario con ese nombre.");
        }
    }

    /// <summary>
    /// Obtiene todos los procedimientos documentarios activos de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todos los procedimientos documentarios activos.</returns>
    public async Task<List<DocumentaryProcedureDto>> GetAllAsync()
    {
        var procedures = await _context.DocumentaryProcedures
            .Include(p => p.Area)
            .Where(p => p.Status)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var result = new List<DocumentaryProcedureDto>();

        foreach (var procedure in procedures)
        {
            var procedureDto = new DocumentaryProcedureDto
            {
                Id = procedure.Id,
                Name = procedure.Name,
                Description = procedure.Description,
                Status = procedure.Status,
                AreaId = procedure.AreaId,
                AreaName = procedure.Area.Name,
                CreatedAt = procedure.CreatedAt,
                UpdatedAt = procedure.UpdatedAt,
                Steps = await GetStepsByProcedureIdAsync(procedure.Id)
            };

            result.Add(procedureDto);
        }

        return result;
    }

    /// <summary>
    /// Obtiene un procedimiento documentario específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del procedimiento documentario.</param>
    /// <returns>Un objeto DTO que representa el procedimiento documentario.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el procedimiento documentario no se encuentra.</exception>
    public async Task<DocumentaryProcedureDto> GetByIdAsync(int id)
    {
        var procedure = await _context.DocumentaryProcedures
            .Include(p => p.Area)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (procedure == null)
        {
            throw new InvalidOperationException("Documentary procedure not found.");
        }

        var procedureDto = new DocumentaryProcedureDto
        {
            Id = procedure.Id,
            Name = procedure.Name,
            Description = procedure.Description,
            Status = procedure.Status,
            AreaId = procedure.AreaId,
            AreaName = procedure.Area.Name,
            CreatedAt = procedure.CreatedAt,
            UpdatedAt = procedure.UpdatedAt,
            Steps = await GetStepsByProcedureIdAsync(procedure.Id)
        };

        return procedureDto;
    }

    /// <summary>
    /// Actualiza un procedimiento documentario existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del procedimiento.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentException">Se lanza cuando el ID es requerido para la operación de actualización.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el procedimiento no se encuentra o ya existe otro con el mismo nombre.</exception>
    public async Task UpdateAsync(DocumentaryProcedureRequestParams requestParams)
    {
        if (!requestParams.Id.HasValue)
        {
            throw new ArgumentException("Id is required for update operation.");
        }

        var existingProcedure = await _context.DocumentaryProcedures
            .FirstOrDefaultAsync(p => p.Id == requestParams.Id.Value);

        if (existingProcedure == null)
        {
            throw new InvalidOperationException("Documentary procedure not found.");
        }

        if (existingProcedure.Name != requestParams.Name && !await IsProcedureNameUniqueAsync(requestParams.Name, requestParams.Id.Value))
        {
            throw new InvalidOperationException("Ya existe otro trámite documentario con ese nombre.");
        }

        existingProcedure.Name = requestParams.Name;
        existingProcedure.Description = requestParams.Description;
        existingProcedure.Status = requestParams.Status;
        existingProcedure.AreaId = requestParams.AreaId;
        existingProcedure.UpdatedAt = DateTime.UtcNow;

        await DeleteExistingStepsAsync(requestParams.Id.Value);
        await CreateStepsAsync(requestParams.Id.Value, requestParams.Steps);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Elimina lógicamente un procedimiento documentario por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del procedimiento documentario a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el procedimiento documentario no se encuentra.</exception>
    public async Task DeleteAsync(int id)
    {
        var procedure = await _context.DocumentaryProcedures
            .FirstOrDefaultAsync(p => p.Id == id);

        if (procedure == null)
        {
            throw new InvalidOperationException("Documentary procedure not found.");
        }

        procedure.IsDeleted = true;
        procedure.DeletedAt = DateTime.UtcNow;
        procedure.UpdatedAt = DateTime.UtcNow;

        await DeleteExistingStepsLogicallyAsync(id);
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Verifica si el nombre de un procedimiento documentario es único de forma asíncrona.
    /// </summary>
    /// <param name="name">El nombre del procedimiento a verificar.</param>
    /// <param name="excludeId">ID del procedimiento a excluir de la verificación (opcional).</param>
    /// <returns>True si el nombre es único, false en caso contrario.</returns>
    private async Task<bool> IsProcedureNameUniqueAsync(string name, int? excludeId = null)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        var query = _context.DocumentaryProcedures
            .Where(p => p.Name.Trim().ToLower() == trimmedName.ToLower());
            
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
            
        return !await query.AnyAsync();
    }

    /// <summary>
    /// Crea los pasos de un procedimiento documentario de forma asíncrona.
    /// </summary>
    /// <param name="procedureId">El identificador del procedimiento documentario.</param>
    /// <param name="steps">Lista de parámetros de pasos a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    private async Task CreateStepsAsync(int procedureId, List<DocumentaryProcedureStepRequestParams> steps)
    {
        if (steps == null || !steps.Any()) return;

        foreach (var stepRequest in steps)
        {
            var step = new DocumentaryProcedureStep
            {
                DocumentaryProcedureId = procedureId,
                AreaId = stepRequest.AreaId,
                PositionId = stepRequest.PositionId,
                Order = stepRequest.Order
            };

            _context.DocumentaryProcedureSteps.Add(step);
            await _context.SaveChangesAsync();

            await CreateStepDocumentsAsync(step.Id, stepRequest.Documents);
        }
    }

    /// <summary>
    /// Crea los documentos asociados a un paso de procedimiento de forma asíncrona.
    /// </summary>
    /// <param name="stepId">El identificador del paso del procedimiento.</param>
    /// <param name="documents">Lista de parámetros de documentos a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    private async Task CreateStepDocumentsAsync(int stepId, List<DocumentaryProcedureStepDocumentRequestParams> documents)
    {
        if (documents == null || !documents.Any()) return;

        foreach (var documentRequest in documents)
        {
            var stepDocument = new DocumentaryProcedureStepDocument
            {
                DocumentaryProcedureStepId = stepId,
                DocumentTypeId = documentRequest.DocumentTypeId,
                RequiresSignature = documentRequest.RequiresSignature
            };

            _context.DocumentaryProcedureStepDocuments.Add(stepDocument);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Elimina físicamente los pasos existentes de un procedimiento de forma asíncrona.
    /// </summary>
    /// <param name="procedureId">El identificador del procedimiento documentario.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    private async Task DeleteExistingStepsAsync(int procedureId)
    {
        var existingSteps = await _context.DocumentaryProcedureSteps
            .Where(s => s.DocumentaryProcedureId == procedureId)
            .ToListAsync();

        foreach (var step in existingSteps)
        {
            var existingDocuments = await _context.DocumentaryProcedureStepDocuments
                .Where(d => d.DocumentaryProcedureStepId == step.Id)
                .ToListAsync();

            _context.DocumentaryProcedureStepDocuments.RemoveRange(existingDocuments);
        }

        _context.DocumentaryProcedureSteps.RemoveRange(existingSteps);
    }

    /// <summary>
    /// Elimina lógicamente los pasos existentes de un procedimiento de forma asíncrona.
    /// </summary>
    /// <param name="procedureId">El identificador del procedimiento documentario.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    private async Task DeleteExistingStepsLogicallyAsync(int procedureId)
    {
        var existingSteps = await _context.DocumentaryProcedureSteps
            .Where(s => s.DocumentaryProcedureId == procedureId && !s.IsDeleted)
            .ToListAsync();

        foreach (var step in existingSteps)
        {
            var existingDocuments = await _context.DocumentaryProcedureStepDocuments
                .Where(d => d.DocumentaryProcedureStepId == step.Id && !d.IsDeleted)
                .ToListAsync();

            foreach (var doc in existingDocuments)
            {
                doc.IsDeleted = true;
                doc.DeletedAt = DateTime.UtcNow;
                doc.UpdatedAt = DateTime.UtcNow;
            }

            step.IsDeleted = true;
            step.DeletedAt = DateTime.UtcNow;
            step.UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Obtiene los pasos de un procedimiento documentario por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="procedureId">El identificador del procedimiento documentario.</param>
    /// <returns>Una lista de objetos DTO que representan los pasos del procedimiento.</returns>
    private async Task<List<DocumentaryProcedureStepDto>> GetStepsByProcedureIdAsync(int procedureId)
    {
        var steps = await _context.DocumentaryProcedureSteps
            .Include(s => s.Area)
            .Include(s => s.Position)
            .Where(s => s.DocumentaryProcedureId == procedureId)
            .OrderBy(s => s.Order)
            .ToListAsync();

        var stepDtos = new List<DocumentaryProcedureStepDto>();

        foreach (var step in steps)
        {
            var stepDto = new DocumentaryProcedureStepDto
            {
                Id = step.Id,
                DocumentaryProcedureId = step.DocumentaryProcedureId,
                AreaId = step.AreaId,
                AreaName = step.Area.Name,
                PositionId = step.PositionId,
                PositionName = step.Position.Name,
                Order = step.Order,
                CreatedAt = step.CreatedAt,
                UpdatedAt = step.UpdatedAt,
                Documents = await GetDocumentsByStepIdAsync(step.Id)
            };

            stepDtos.Add(stepDto);
        }

        return stepDtos;
    }

    /// <summary>
    /// Obtiene los documentos asociados a un paso de procedimiento por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="stepId">El identificador del paso del procedimiento.</param>
    /// <returns>Una lista de objetos DTO que representan los documentos del paso.</returns>
    private async Task<List<DocumentaryProcedureStepDocumentDto>> GetDocumentsByStepIdAsync(int stepId)
    {
        var documents = await _context.DocumentaryProcedureStepDocuments
            .Include(d => d.DocumentType)
            .Where(d => d.DocumentaryProcedureStepId == stepId)
            .ToListAsync();

        return documents.Select(d => new DocumentaryProcedureStepDocumentDto
        {
            Id = d.Id,
            DocumentaryProcedureStepId = d.DocumentaryProcedureStepId,
            DocumentTypeId = d.DocumentTypeId,
            DocumentTypeName = d.DocumentType.Name,
            IsUploadable = d.DocumentType.IsUploadable,
            RequiresSignature = d.RequiresSignature,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        }).ToList();
    }
}