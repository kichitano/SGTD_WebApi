using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.DocumentType;

namespace SGTD_WebApi.Services.Implementation;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly DatabaseContext _context;
    public DocumentTypeService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(DocumentTypeRequestParams requestParams)
    {
        var trimmedName = requestParams.Name?.Trim() ?? string.Empty;
        var existingDocumentType = await _context.DocumentTypes
            .AnyAsync(dt => dt.Name.Trim().ToLower() == trimmedName.ToLower());
            
        if (existingDocumentType)
        {
            throw new InvalidOperationException("Ya existe un tipo de documento con ese nombre.");
        }

        var documentType = new DocumentType
        {
            Name = requestParams.Name,
            IsUploadable = requestParams.IsUploadable,
        };
        _context.DocumentTypes.Add(documentType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DocumentTypeRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "DocumentType Id is required for update.");
        var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(c => c.Id == requestParams.Id);
        if (documentType == null)
            throw new KeyNotFoundException("DocumentType not found.");

        var trimmedName = requestParams.Name?.Trim() ?? string.Empty;
        var existingDocumentType = await _context.DocumentTypes
            .AnyAsync(dt => dt.Id != requestParams.Id && dt.Name.Trim().ToLower() == trimmedName.ToLower());
            
        if (existingDocumentType)
        {
            throw new InvalidOperationException("Ya existe otro tipo de documento con ese nombre.");
        }

        documentType.Name = requestParams.Name;
        documentType.IsUploadable = requestParams.IsUploadable;
        documentType.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<DocumentTypeDto>> GetAllAsync()
    {
        return await _context.DocumentTypes
            .Select(q => new DocumentTypeDto
            {
                Id = q.Id,
                Name = q.Name,
                IsUploadable = q.IsUploadable,
            })
            .ToListAsync();
    }

    public async Task<DocumentTypeDto> GetByIdAsync(int id)
    {
        var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(q => q.Id == id);
        if (documentType == null)
            throw new KeyNotFoundException("DocumentType not found.");
        return new DocumentTypeDto
        {
            Id = documentType.Id,
            Name = documentType.Name,
            IsUploadable = documentType.IsUploadable
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var documentType = await _context.DocumentTypes.FirstOrDefaultAsync(q => q.Id == id);
        if (documentType == null)
            throw new KeyNotFoundException("DocumentType not found.");

        var hasActiveProcedureSteps = await _context.DocumentaryProcedureStepDocuments.AnyAsync(dpsd => dpsd.DocumentTypeId == id && !dpsd.IsDeleted);
        if (hasActiveProcedureSteps)
        {
            throw new InvalidOperationException("No se puede eliminar el tipo de documento porque está siendo usado por procedimientos documentarios activos.");
        }

        documentType.IsDeleted = true;
        documentType.DeletedAt = DateTime.UtcNow;
        documentType.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}