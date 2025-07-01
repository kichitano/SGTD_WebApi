using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Position;
using SGTD_WebApi.Models.PositionDependency;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar cargos o posiciones organizacionales.
/// Proporciona funcionalidades completas para crear, actualizar, consultar y eliminar posiciones,
/// incluyendo la gestión de jerarquías, dependencias y validaciones de integridad organizacional.
/// </summary>
public class PositionService : IPositionService
{
    private readonly DatabaseContext _context;
    private readonly IPositionDependencyService _positionDependencyService;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de posiciones.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    /// <param name="positionDependencyService">Servicio para gestionar dependencias entre posiciones.</param>
    public PositionService(
        DatabaseContext context,
        IPositionDependencyService positionDependencyService)
    {
        _context = context;
        _positionDependencyService = positionDependencyService;
    }

    /// <summary>
    /// Crea una nueva posición de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información de la posición a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el nombre de la posición ya existe o hay conflictos de jerarquía.</exception>
    public async Task CreateAsync(PositionRequestParams requestParams)
    {
        if (await IsPositionNameUniqueAsync(requestParams.Name))
        {
            await ValidateDirectManagerLogicAsync(requestParams.AreaId, requestParams.DirectManagerPositionId);

            var position = new Position
            {
                Name = requestParams.Name,
                Description = requestParams.Description,
                AreaId = requestParams.AreaId,
                DirectManagerPositionId = requestParams.DirectManagerPositionId
            };

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            if (requestParams.ParentPositionId.HasValue)
            {
                var dependencyRequest = new PositionDependencyRequestParams
                {
                    ParentPositionId = requestParams.ParentPositionId.Value,
                    ChildPositionId = position.Id
                };
                await _positionDependencyService.CreateAsync(dependencyRequest);
            }
        }
        else
        {
            throw new InvalidOperationException("Position name already exists.");
        }
    }

    /// <summary>
    /// Actualiza una posición existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada de la posición.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentNullException">Se lanza cuando el ID de la posición es nulo.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la posición no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando el nombre ya existe o hay conflictos de jerarquía.</exception>
    public async Task UpdateAsync(PositionRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Position Id is required for update.");

        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == requestParams.Id);
        if (position == null)
            throw new KeyNotFoundException("Position not found.");

        if (await IsPositionNameUniqueAsync(requestParams.Name, requestParams.Id))
        {
            await ValidateDirectManagerLogicAsync(requestParams.AreaId, requestParams.DirectManagerPositionId, requestParams.Id);

            position.Name = requestParams.Name;
            position.Description = requestParams.Description;
            position.AreaId = requestParams.AreaId;
            position.DirectManagerPositionId = requestParams.DirectManagerPositionId;
            await _context.SaveChangesAsync();

            var existingDependency = await _context.PositionsDependency
                .FirstOrDefaultAsync(pd => pd.ChildPositionId == requestParams.Id);

            if (requestParams.ParentPositionId.HasValue)
            {
                if (existingDependency != null)
                {
                    var updateRequest = new PositionDependencyRequestParams
                    {
                        Id = existingDependency.Id,
                        ParentPositionId = requestParams.ParentPositionId.Value,
                        ChildPositionId = position.Id
                    };
                    await _positionDependencyService.UpdateAsync(updateRequest);
                }
                else
                {
                    var createRequest = new PositionDependencyRequestParams
                    {
                        ParentPositionId = requestParams.ParentPositionId.Value,
                        ChildPositionId = position.Id
                    };
                    await _positionDependencyService.CreateAsync(createRequest);
                }
            }
            else if (existingDependency != null)
            {
                await _positionDependencyService.DeleteByIdAsync(existingDependency.Id);
            }
        }
        else
        {
            throw new InvalidOperationException("Position name already exists.");
        }
    }

    /// <summary>
    /// Obtiene todas las posiciones de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todas las posiciones.</returns>
    public async Task<List<PositionDto>> GetAllAsync()
    {
        var positions = await _context.Positions
            .Include(p => p.Area)
            .Include(p => p.DirectManagerPosition)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                AreaId = p.AreaId,
                AreaName = p.Area.Name,
                DirectManagerPositionId = p.DirectManagerPositionId,
                DirectManagerPositionName = p.DirectManagerPosition != null ? p.DirectManagerPosition.Name : null,
                ParentPositionId = _context.PositionsDependency
                    .Where(pd => pd.ChildPositionId == p.Id)
                    .Select(pd => pd.ParentPositionId)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return positions;
    }

    /// <summary>
    /// Obtiene una posición específica por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la posición.</param>
    /// <returns>Un objeto DTO que representa la posición.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la posición no se encuentra.</exception>
    public async Task<PositionDto> GetByIdAsync(int id)
    {
        var position = await _context.Positions
            .Include(p => p.Area)
            .Include(p => p.DirectManagerPosition)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                AreaId = p.AreaId,
                AreaName = p.Area.Name,
                DirectManagerPositionId = p.DirectManagerPositionId,
                DirectManagerPositionName = p.DirectManagerPosition != null ? p.DirectManagerPosition.Name : null,
                ParentPositionId = _context.PositionsDependency
                    .Where(pd => pd.ChildPositionId == p.Id)
                    .Select(pd => pd.ParentPositionId)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(p => p.Id == id);

        if (position == null)
            throw new KeyNotFoundException("Position not found.");

        return position;
    }

    /// <summary>
    /// Elimina una posición por su identificador de forma asíncrona.
    /// Verifica que la posición no tenga usuarios ni pasos de procedimientos activos asociados.
    /// </summary>
    /// <param name="id">El identificador único de la posición a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la posición no se encuentra.</exception>
    /// <exception cref="InvalidOperationException">Se lanza cuando la posición tiene usuarios o pasos de procedimientos activos.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
        if (position == null)
            throw new KeyNotFoundException("Position not found.");

        var hasActiveUsers = await _context.Users.AnyAsync(u => u.PositionId == id && !u.IsDeleted);
        if (hasActiveUsers)
        {
            throw new InvalidOperationException("Cannot delete position because it is assigned to one or more active users. Please reassign users to different positions first.");
        }

        var hasActiveProcedureSteps = await _context.DocumentaryProcedureSteps.AnyAsync(dps => dps.PositionId == id && !dps.IsDeleted);
        if (hasActiveProcedureSteps)
        {
            throw new InvalidOperationException("Cannot delete position because it is referenced by one or more active documentary procedure steps. Please update or remove these procedure steps first.");
        }

        position.IsDeleted = true;
        position.DeletedAt = DateTime.UtcNow;
        position.UpdatedAt = DateTime.UtcNow;

        var dependencies = await _context.PositionsDependency
            .Where(pd => (pd.ParentPositionId == id || pd.ChildPositionId == id) && !pd.IsDeleted)
            .ToListAsync();
        
        foreach (var dep in dependencies)
        {
            dep.IsDeleted = true;
            dep.DeletedAt = DateTime.UtcNow;
            dep.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Crea una nueva posición y retorna su identificador de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información de la posición a crear.</param>
    /// <returns>El identificador de la posición creada.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el nombre de la posición ya existe.</exception>
    public async Task<int> CreateReturnIdAsync(PositionRequestParams requestParams)
    {
        if (await IsPositionNameUniqueAsync(requestParams.Name))
        {
            var position = new Position
            {
                Name = requestParams.Name,
                Description = requestParams.Description,
                AreaId = requestParams.AreaId
            };

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            if (requestParams.ParentPositionId.HasValue)
            {
                var dependencyRequest = new PositionDependencyRequestParams
                {
                    ParentPositionId = requestParams.ParentPositionId.Value,
                    ChildPositionId = position.Id
                };
                await _positionDependencyService.CreateAsync(dependencyRequest);
            }

            return position.Id;
        }

        throw new InvalidOperationException("Position name already exists.");
    }

    /// <summary>
    /// Obtiene todas las posiciones de un área específica de forma asíncrona.
    /// </summary>
    /// <param name="areaId">El identificador del área.</param>
    /// <returns>Una lista de objetos DTO que representan las posiciones del área.</returns>
    public async Task<List<PositionDto>> GetAllByAreaIdAsync(int areaId)
    {
        var positions = await _context.Positions
            .Where(q => q.Area.Id == areaId)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                AreaId = p.AreaId,
                ParentPositionId = _context.PositionsDependency
                    .Where(pd => pd.ChildPositionId == p.Id)
                    .Select(pd => pd.ParentPositionId)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return positions;
    }

    /// <summary>
    /// Obtiene las posiciones disponibles para ser jefes directos en un área de forma asíncrona.
    /// </summary>
    /// <param name="currentAreaId">El identificador del área actual.</param>
    /// <param name="excludePositionId">ID de la posición a excluir de los resultados (opcional).</param>
    /// <returns>Una lista de objetos DTO que representan las posiciones disponibles como jefes directos.</returns>
    public async Task<List<PositionDto>> GetAvailableDirectManagersAsync(int currentAreaId, int? excludePositionId = null)
    {
        var sameAreaPositions = await _context.Positions
            .Include(p => p.Area)
            .Where(p => p.AreaId == currentAreaId && (!excludePositionId.HasValue || p.Id != excludePositionId.Value) && !p.IsDeleted)
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                AreaId = p.AreaId,
                AreaName = p.Area.Name
            })
            .ToListAsync();

        return sameAreaPositions.OrderBy(p => p.Name).ToList();
    }

    public async Task<bool> AreaHasMaxAuthorityAsync(int areaId, int? excludePositionId = null)
    {
        var query = _context.Positions
            .Where(p => p.AreaId == areaId && p.DirectManagerPositionId == null && !p.IsDeleted);

        if (excludePositionId.HasValue)
            query = query.Where(p => p.Id != excludePositionId.Value);

        return await query.AnyAsync();
    }

    public async Task<PositionDto?> GetMaxAuthorityByAreaAsync(int areaId, int? excludePositionId = null)
    {
        var query = _context.Positions
            .Include(p => p.Area)
            .Where(p => p.AreaId == areaId && p.DirectManagerPositionId == null && !p.IsDeleted);

        if (excludePositionId.HasValue)
            query = query.Where(p => p.Id != excludePositionId.Value);

        var maxAuthority = await query
            .Select(p => new PositionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                AreaId = p.AreaId,
                AreaName = p.Area.Name,
                DirectManagerPositionId = p.DirectManagerPositionId
            })
            .FirstOrDefaultAsync();

        return maxAuthority;
    }

    private async Task<bool> IsPositionNameUniqueAsync(string name, int? excludePositionId = null)
    {
        var query = _context.Positions
            .Where(p => p.Name == name && !p.IsDeleted);

        if (excludePositionId.HasValue)
            query = query.Where(p => p.Id != excludePositionId.Value);

        return !await query.AnyAsync();
    }

    private async Task ValidateDirectManagerLogicAsync(int areaId, int? directManagerPositionId, int? excludePositionId = null)
    {
        if (!directManagerPositionId.HasValue)
        {
            var query = _context.Positions
                .Where(p => p.AreaId == areaId && p.DirectManagerPositionId == null && !p.IsDeleted);

            if (excludePositionId.HasValue)
                query = query.Where(p => p.Id != excludePositionId.Value);

            var existingMaxAuthority = await query.AnyAsync();
            
            if (existingMaxAuthority)
            {
                throw new InvalidOperationException("Ya existe una máxima autoridad (cargo sin jefe inmediato) para esta área. Solo puede haber un cargo sin jefe inmediato por área.");
            }
        }
        else
        {
            var directManager = await _context.Positions
                .FirstOrDefaultAsync(p => p.Id == directManagerPositionId.Value && !p.IsDeleted);

            if (directManager == null)
            {
                throw new InvalidOperationException("El jefe directo especificado no existe.");
            }

            if (excludePositionId.HasValue && await WouldCreateCircularReferenceAsync(excludePositionId.Value, directManagerPositionId.Value))
            {
                throw new InvalidOperationException("La asignación de jefe directo crearía una referencia circular en la jerarquía.");
            }
        }
    }

    private async Task<bool> WouldCreateCircularReferenceAsync(int positionId, int proposedManagerId)
    {
        var currentManagerId = (int?)proposedManagerId;
        var visitedIds = new HashSet<int> { positionId };

        while (currentManagerId.HasValue)
        {
            if (visitedIds.Contains(currentManagerId.Value))
                return true;

            visitedIds.Add(currentManagerId.Value);

            var manager = await _context.Positions
                .FirstOrDefaultAsync(p => p.Id == currentManagerId.Value && !p.IsDeleted);

            currentManagerId = manager?.DirectManagerPositionId;
        }

        return false;
    }
}