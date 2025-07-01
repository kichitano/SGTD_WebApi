using SGTD_WebApi.Models.DocumentType;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de tipos de documento.
/// </summary>
public interface IDocumentTypeService
{
    /// <summary>
    /// Crea un nuevo tipo de documento.
    /// </summary>
    /// <param name="requestParams">Parámetros del nuevo tipo de documento.</param>
    Task CreateAsync(DocumentTypeRequestParams requestParams);
    /// <summary>
    /// Actualiza un tipo de documento existente.
    /// </summary>
    /// <param name="requestParams">Parámetros de actualización del tipo de documento.</param>
    Task UpdateAsync(DocumentTypeRequestParams requestParams);
    /// <summary>
    /// Obtiene todos los tipos de documento.
    /// </summary>
    /// <returns>Lista de tipos de documento.</returns>
    Task<List<DocumentTypeDto>> GetAllAsync();
    /// <summary>
    /// Obtiene un tipo de documento por su identificador.
    /// </summary>
    /// <param name="id">Identificador del tipo de documento.</param>
    /// <returns>Tipo de documento encontrado.</returns>
    Task<DocumentTypeDto> GetByIdAsync(int id);
    /// <summary>
    /// Elimina un tipo de documento del sistema.
    /// </summary>
    /// <param name="id">Identificador del tipo de documento a eliminar.</param>
    Task DeleteByIdAsync(int id);
}