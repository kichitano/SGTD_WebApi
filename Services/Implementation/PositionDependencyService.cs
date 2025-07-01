using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.PositionDependency;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar las dependencias entre cargos o posiciones.
/// Permite crear, actualizar, consultar y eliminar relaciones jerárquicas entre posiciones.
/// </summary>
public class PositionDependencyService : IPositionDependencyService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de dependencias de posiciones.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public PositionDependencyService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea una nueva dependencia entre posiciones de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen los identificadores de la posición padre e hija.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task CreateAsync(PositionDependencyRequestParams requestParams)
    {
        var areaDependency = new PositionDependency
        {
            ParentPositionId = requestParams.ParentPositionId,
            ChildPositionId = requestParams.ChildPositionId
        };
        _context.PositionsDependency.Add(areaDependency);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza una dependencia existente entre posiciones de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen el ID de la dependencia y los nuevos identificadores de posiciones.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID de la dependencia es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la dependencia no se encuentra.</exception>
    public async Task UpdateAsync(PositionDependencyRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "PositionDependency Id is required for update.");

        var areaDependency = await _context.PositionsDependency.FirstOrDefaultAsync(ad => ad.Id == requestParams.Id);
        if (areaDependency == null)
            throw new KeyNotFoundException("PositionDependency not found.");

        areaDependency.ParentPositionId = requestParams.ParentPositionId;
        areaDependency.ChildPositionId = requestParams.ChildPositionId;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene todas las dependencias entre posiciones de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan las dependencias entre posiciones.</returns>
    public async Task<List<PositionDependencyDto>> GetAllAsync()
    {
        return await _context.PositionsDependency
            .Include(ad => ad.ParentPosition)
            .Include(ad => ad.ChildPosition)
            .Select(ad => new PositionDependencyDto
            {
                Id = ad.Id,
                ParentPositionId = ad.ParentPositionId,
                ChildPositionId = ad.ChildPositionId
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una dependencia específica entre posiciones por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la dependencia.</param>
    /// <returns>Un objeto DTO que representa la dependencia entre posiciones.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la dependencia no se encuentra.</exception>
    public async Task<PositionDependencyDto> GetByIdAsync(int id)
    {
        var areaDependency = await _context.PositionsDependency
            .Include(ad => ad.ParentPosition)
            .Include(ad => ad.ChildPosition)
            .FirstOrDefaultAsync(ad => ad.Id == id);

        if (areaDependency == null)
            throw new KeyNotFoundException("PositionDependency not found.");

        return new PositionDependencyDto
        {
            Id = areaDependency.Id,
            ParentPositionId = areaDependency.ParentPositionId,
            ChildPositionId = areaDependency.ChildPositionId
        };
    }

    /// <summary>
    /// Elimina una dependencia entre posiciones por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la dependencia a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la dependencia no se encuentra.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var areaDependency = await _context.PositionsDependency.FirstOrDefaultAsync(ad => ad.Id == id);
        if (areaDependency == null)
            throw new KeyNotFoundException("PositionDependency not found.");

        _context.PositionsDependency.Remove(areaDependency);
        await _context.SaveChangesAsync();
    }
}