using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Area;
using SGTD_WebApi.Models.AreaDependency;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar áreas organizacionales.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar áreas,
/// incluyendo la gestión de dependencias jerárquicas y validaciones de integridad.
/// </summary>
public class AreaService : IAreaService
{
    private readonly DatabaseContext _context;
    private readonly IAreaDependencyService _areaDependencyService;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de áreas.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    /// <param name="areaDependencyService">Servicio para gestionar dependencias entre áreas.</param>
    public AreaService(DatabaseContext context, IAreaDependencyService areaDependencyService)
    {
        _context = context;
        _areaDependencyService = areaDependencyService;
    }

    /// <summary>
    /// Crea una nueva área de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información del área a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el nombre del área ya existe.</exception>
    public async Task CreateAsync(AreaRequestParams requestParams)
    {
        if (await IsAreaNameUniqueAsync(requestParams.Name))
        {
            var area = new Area
            {
                Name = requestParams.Name,
                Description = requestParams.Description,
                Status = requestParams.Status
            };

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            if (requestParams.ParentAreaId.HasValue)
            {
                var dependencyRequest = new AreaDependencyRequestParams
                {
                    ParentAreaId = requestParams.ParentAreaId.Value,
                    ChildAreaId = area.Id
                };

                await _areaDependencyService.CreateAsync(dependencyRequest);
            }
        }
        else
        {
            throw new InvalidOperationException("Area name already exists.");
        }
    }

    /// <summary>
    /// Actualiza un área existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada del área.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ValidationException">Se lanza cuando el ID del área es requerido para la actualización.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el área o área padre no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el nombre ya existe o se crearía una referencia circular.</exception>
    public async Task UpdateAsync(AreaRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ValidationException("Area Id is required for update.");

        var area = await _context.Areas.FirstOrDefaultAsync(q => q.Id == requestParams.Id);
        if (area == null)
            throw new KeyNotFoundException("Area not found.");

        if (!await IsAreaNameUniqueAsync(requestParams.Name, requestParams.Id))
            throw new InvalidOperationException("Area name already exists.");

        if (requestParams.ParentAreaId.HasValue)
        {
            var parentArea = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == requestParams.ParentAreaId.Value);
            if (parentArea == null)
                throw new KeyNotFoundException("Parent area not found.");

            if (await WouldCreateCircularReference(requestParams.Id.Value, requestParams.ParentAreaId.Value))
                throw new InvalidOperationException("Cannot create circular reference between areas.");
        }

        area.Name = requestParams.Name;
        area.Description = requestParams.Description;
        area.Status = requestParams.Status;

        var existingDependency = await _context.AreaDependencies
            .FirstOrDefaultAsync(ad => ad.ChildAreaId == requestParams.Id);

        if (requestParams.ParentAreaId.HasValue)
        {
            if (existingDependency != null)
            {
                var updateRequest = new AreaDependencyRequestParams
                {
                    Id = existingDependency.Id,
                    ParentAreaId = requestParams.ParentAreaId.Value,
                    ChildAreaId = area.Id
                };
                await _areaDependencyService.UpdateAsync(updateRequest);
            }
            else
            {
                var createRequest = new AreaDependencyRequestParams
                {
                    ParentAreaId = requestParams.ParentAreaId.Value,
                    ChildAreaId = area.Id
                };
                await _areaDependencyService.CreateAsync(createRequest);
            }
        }
        else if (existingDependency != null)
        {
            await _areaDependencyService.DeleteByIdAsync(existingDependency.Id);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error al actualizar el área: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// Obtiene todas las áreas de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todas las áreas.</returns>
    public async Task<List<AreaDto>> GetAllAsync()
    {
        return await _context.Areas
            .Select(a => new AreaDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Status = a.Status,
                ParentAreaId = _context.AreaDependencies
                    .Where(ad => ad.ChildAreaId == a.Id)
                    .Select(ad => ad.ParentAreaId)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un área específica por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del área.</param>
    /// <returns>Un objeto DTO que representa el área.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el área no se encuentra.</exception>
    public async Task<AreaDto> GetByIdAsync(int id)
    {
        var area = await _context.Areas
            .Select(a => new AreaDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Status = a.Status,
                ParentAreaId = _context.AreaDependencies
                    .Where(ad => ad.ChildAreaId == a.Id)
                    .Select(ad => ad.ParentAreaId)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(a => a.Id == id);

        if (area == null)
            throw new KeyNotFoundException("Area not found.");

        return area;
    }

    /// <summary>
    /// Elimina un área por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del área a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando el área no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el área tiene dependencias o posiciones activas.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var area = await _context.Areas.FirstOrDefaultAsync(q => q.Id == id);
        if (area == null)
            throw new KeyNotFoundException("Area not found.");

        var hasActiveChildAreas = await _context.AreaDependencies
            .AnyAsync(ad => ad.ParentAreaId == id && !ad.IsDeleted);
        
        if (hasActiveChildAreas)
        {
            throw new InvalidOperationException("No se puede eliminar el área porque tiene áreas dependientes activas.");
        }

        var hasActivePositions = await _context.Positions
            .AnyAsync(p => p.AreaId == id && !p.IsDeleted);
            
        if (hasActivePositions)
        {
            throw new InvalidOperationException("No se puede eliminar el área porque tiene posiciones asociadas activas.");
        }

        area.IsDeleted = true;
        area.DeletedAt = DateTime.UtcNow;
        area.UpdatedAt = DateTime.UtcNow;

        var dependencies = await _context.AreaDependencies
            .Where(ad => (ad.ParentAreaId == id || ad.ChildAreaId == id) && !ad.IsDeleted)
            .ToListAsync();
        
        foreach (var dep in dependencies)
        {
            dep.IsDeleted = true;
            dep.DeletedAt = DateTime.UtcNow;
            dep.UpdatedAt = DateTime.UtcNow;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error al eliminar el área: " + ex.Message, ex);
        }
    }
    
    /// <summary>
    /// Verifica si el nombre de un área es único de forma asíncrona.
    /// </summary>
    /// <param name="name">El nombre del área a verificar.</param>
    /// <param name="excludeAreaId">ID del área a excluir de la verificación (opcional).</param>
    /// <returns>True si el nombre es único, false en caso contrario.</returns>
    public async Task<bool> IsAreaNameUniqueAsync(string name, int? excludeAreaId = null)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        var query = _context.Areas
            .Where(a => a.Name.Trim().ToLower() == trimmedName.ToLower());

        if (excludeAreaId.HasValue)
            query = query.Where(a => a.Id != excludeAreaId.Value);

        return !await query.AnyAsync();
    }

    /// <summary>
    /// Verifica si asignar un área padre crearía una referencia circular de forma asíncrona.
    /// </summary>
    /// <param name="targetAreaId">ID del área objetivo.</param>
    /// <param name="parentAreaId">ID del área padre propuesta.</param>
    /// <returns>True si se crearía una referencia circular, false en caso contrario.</returns>
    private async Task<bool> WouldCreateCircularReference(int targetAreaId, int parentAreaId)
    {
        if (targetAreaId == parentAreaId)
            return true;

        var dependencies = await _context.AreaDependencies.ToListAsync();
        var visited = new HashSet<int>();
        var stack = new Stack<int>();

        stack.Push(parentAreaId);

        while (stack.Count > 0)
        {
            var currentAreaId = stack.Pop();

            if (!visited.Add(currentAreaId))
                continue;

            var parentAreas = dependencies
                .Where(d => d.ChildAreaId == currentAreaId)
                .Select(d => d.ParentAreaId);

            foreach (var area in parentAreas)
            {
                if (area == targetAreaId)
                    return true;

                stack.Push(area);
            }
        }

        return false;
    }
}