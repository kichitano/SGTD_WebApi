using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.DocumentType;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar tipos de documentos del sistema.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar tipos de documentos,
/// con validación de nombres únicos y control de dependencias con procedimientos documentarios.
/// </summary>
public class DocumentTypeService : IDocumentTypeService
{
    private readonly DatabaseContext _context;
    /// <summary>
    /// Inicializa una nueva instancia del servicio de tipos de documentos.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public DocumentTypeService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un nuevo tipo de documento de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del tipo de documento a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe un tipo de documento con el mismo nombre.</exception>
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

    /// <summary>
    /// Actualiza un tipo de documento existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del tipo de documento.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID del tipo de documento es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el tipo de documento no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando ya existe otro tipo de documento con el mismo nombre.</exception>
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

    /// <summary>
    /// Obtiene todos los tipos de documentos de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todos los tipos de documentos.</returns>
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

    /// <summary>
    /// Obtiene un tipo de documento específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del tipo de documento.</param>
    /// <returns>Un objeto DTO que representa el tipo de documento.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el tipo de documento no se encuentra.</exception>
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

    /// <summary>
    /// Elimina un tipo de documento por su identificador de forma asíncrona.
    /// Verifica que el tipo de documento no esté siendo usado por procedimientos documentarios activos.
    /// </summary>
    /// <param name="id">El identificador único del tipo de documento a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el tipo de documento no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el tipo de documento está siendo usado por procedimientos activos.</exception>
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