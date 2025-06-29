using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Position;
using SGTD_WebApi.Models.PositionDependency;

namespace SGTD_WebApi.Services.Implementation;

public class PositionService : IPositionService
{
    private readonly DatabaseContext _context;
    private readonly IPositionDependencyService _positionDependencyService;
    private readonly IUserRoleService _userRoleService;

    public PositionService(
        DatabaseContext context,
        IPositionDependencyService positionDependencyService,
        IUserRoleService userRoleService)
    {
        _context = context;
        _positionDependencyService = positionDependencyService;
        _userRoleService = userRoleService;
    }

    public async Task CreateAsync(PositionRequestParams requestParams)
    {
        if (await IsPositionNameUniqueAsync(requestParams.Name))
        {
            // Validar la lógica de jefatura directa
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

    public async Task UpdateAsync(PositionRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Position Id is required for update.");

        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == requestParams.Id);
        if (position == null)
            throw new KeyNotFoundException("Position not found.");

        if (await IsPositionNameUniqueAsync(requestParams.Name, requestParams.Id))
        {
            // Validar la lógica de jefatura directa (excluyendo la posición actual)
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

    public async Task DeleteByIdAsync(int id)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
        if (position == null)
            throw new KeyNotFoundException("Position not found.");

        // Check if position is referenced by any active users
        var hasActiveUsers = await _context.Users.AnyAsync(u => u.PositionId == id && !u.IsDeleted);
        if (hasActiveUsers)
        {
            throw new InvalidOperationException("Cannot delete position because it is assigned to one or more active users. Please reassign users to different positions first.");
        }

        // Check if position is referenced by any active documentary procedure steps
        var hasActiveProcedureSteps = await _context.DocumentaryProcedureSteps.AnyAsync(dps => dps.PositionId == id && !dps.IsDeleted);
        if (hasActiveProcedureSteps)
        {
            throw new InvalidOperationException("Cannot delete position because it is referenced by one or more active documentary procedure steps. Please update or remove these procedure steps first.");
        }

        // Realizar eliminación lógica en lugar de física
        position.IsDeleted = true;
        position.DeletedAt = DateTime.UtcNow;
        position.UpdatedAt = DateTime.UtcNow;

        // También realizar eliminación lógica de dependencias donde esta posición es padre o hijo
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

    public async Task<List<PositionDto>> GetAvailableDirectManagersAsync(int currentAreaId, int? excludePositionId = null)
    {
        // Solo obtener posiciones de la misma área (excluyendo la posición actual si se especifica)
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
        // Si no se especifica jefe directo, verificar que no exista ya una máxima autoridad para esta área
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
            // Si se especifica jefe directo, verificar que pertenezca a la misma área o a otra área válida
            var directManager = await _context.Positions
                .FirstOrDefaultAsync(p => p.Id == directManagerPositionId.Value && !p.IsDeleted);

            if (directManager == null)
            {
                throw new InvalidOperationException("El jefe directo especificado no existe.");
            }

            // Verificar que no se cree una referencia circular
            if (excludePositionId.HasValue && await WouldCreateCircularReferenceAsync(excludePositionId.Value, directManagerPositionId.Value))
            {
                throw new InvalidOperationException("La asignación de jefe directo crearía una referencia circular en la jerarquía.");
            }
        }
    }

    private async Task<bool> WouldCreateCircularReferenceAsync(int positionId, int proposedManagerId)
    {
        // Verificar si el proposed manager tiene como jefe (directo o indirecto) a la posición actual
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