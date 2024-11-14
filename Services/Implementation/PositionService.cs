using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
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
            position.Name = requestParams.Name;
            position.Description = requestParams.Description;
            position.AreaId = requestParams.AreaId;
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

    public async Task<PositionDto> GetByIdAsync(int id)
    {
        var position = await _context.Positions
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

        _context.Positions.Remove(position);
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

    private async Task<bool> IsPositionNameUniqueAsync(string name, int? excludePositionId = null)
    {
        var query = _context.Positions
            .Where(p => p.Name == name);

        if (excludePositionId.HasValue)
            query = query.Where(p => p.Id != excludePositionId.Value);

        return !await query.AnyAsync();
    }
}