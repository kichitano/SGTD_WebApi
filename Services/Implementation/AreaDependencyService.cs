using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.AreaDependency;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar las dependencias entre áreas organizacionales.
/// Permite crear, actualizar, consultar y eliminar relaciones jerárquicas entre áreas.
/// </summary>
public class AreaDependencyService : IAreaDependencyService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de dependencias de áreas.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public AreaDependencyService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea una nueva dependencia entre áreas de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen los identificadores del área padre e hija.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task CreateAsync(AreaDependencyRequestParams requestParams)
    {
        var areaDependency = new AreaDependency
        {
            ParentAreaId = requestParams.ParentAreaId,
            ChildAreaId = requestParams.ChildAreaId
        };
        _context.AreaDependencies.Add(areaDependency);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza una dependencia existente entre áreas de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen el ID de la dependencia y los nuevos identificadores de áreas.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID de la dependencia es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la dependencia no se encuentra.</exception>
    public async Task UpdateAsync(AreaDependencyRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "AreaDependency Id is required for update.");

        var areaDependency = await _context.AreaDependencies.FirstOrDefaultAsync(ad => ad.Id == requestParams.Id);
        if (areaDependency == null)
            throw new KeyNotFoundException("AreaDependency not found.");

        areaDependency.ParentAreaId = requestParams.ParentAreaId;
        areaDependency.ChildAreaId = requestParams.ChildAreaId;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene todas las dependencias entre áreas de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan las dependencias entre áreas.</returns>
    public async Task<List<AreaDependencyDto>> GetAllAsync()
    {
        return await _context.AreaDependencies
            .Include(ad => ad.ParentArea)
            .Include(ad => ad.ChildArea)
            .Select(ad => new AreaDependencyDto
            {
                Id = ad.Id,
                ParentAreaId = ad.ParentAreaId,
                ChildAreaId = ad.ChildAreaId
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una dependencia específica entre áreas por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la dependencia.</param>
    /// <returns>Un objeto DTO que representa la dependencia entre áreas.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la dependencia no se encuentra.</exception>
    public async Task<AreaDependencyDto> GetByIdAsync(int id)
    {
        var areaDependency = await _context.AreaDependencies
            .Include(ad => ad.ParentArea)
            .Include(ad => ad.ChildArea)
            .FirstOrDefaultAsync(ad => ad.Id == id);

        if (areaDependency == null)
            throw new KeyNotFoundException("AreaDependency not found.");

        return new AreaDependencyDto
        {
            Id = areaDependency.Id,
            ParentAreaId = areaDependency.ParentAreaId,
            ChildAreaId = areaDependency.ChildAreaId
        };
    }

    /// <summary>
    /// Elimina una dependencia entre áreas por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la dependencia a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la dependencia no se encuentra.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var areaDependency = await _context.AreaDependencies.FirstOrDefaultAsync(ad => ad.Id == id);
        if (areaDependency == null)
            throw new KeyNotFoundException("AreaDependency not found.");

        _context.AreaDependencies.Remove(areaDependency);
        await _context.SaveChangesAsync();
    }
}