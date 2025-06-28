using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.DocumentaryProcedure;

namespace SGTD_WebApi.Services.Implementation;

public class DocumentaryProcedureService : IDocumentaryProcedureService
{
    private readonly DatabaseContext _context;

    public DocumentaryProcedureService(DatabaseContext context)
    {
        _context = context;
    }

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
            throw new InvalidOperationException("Documentary procedure name already exists.");
        }
    }

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

        if (existingProcedure.Name != requestParams.Name && !await IsProcedureNameUniqueAsync(requestParams.Name))
        {
            throw new InvalidOperationException("Documentary procedure name already exists.");
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

    public async Task DeleteAsync(int id)
    {
        var procedure = await _context.DocumentaryProcedures
            .FirstOrDefaultAsync(p => p.Id == id);

        if (procedure == null)
        {
            throw new InvalidOperationException("Documentary procedure not found.");
        }

        await DeleteExistingStepsAsync(id);
        _context.DocumentaryProcedures.Remove(procedure);
        await _context.SaveChangesAsync();
    }

    private async Task<bool> IsProcedureNameUniqueAsync(string name)
    {
        return !await _context.DocumentaryProcedures
            .AnyAsync(p => p.Name.ToLower() == name.ToLower());
    }

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