using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.Area;
using SGTD_WebApi.Models.AreaDependency;

namespace SGTD_WebApi.Services.Implementation;

public class AreaService : IAreaService
{
    private readonly DatabaseContext _context;
    private readonly IAreaDependencyService _areaDependencyService;

    public AreaService(DatabaseContext context, IAreaDependencyService areaDependencyService)
    {
        _context = context;
        _areaDependencyService = areaDependencyService;
    }

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

        await _context.SaveChangesAsync();
    }

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

    public async Task DeleteByIdAsync(int id)
    {
        var area = await _context.Areas.FirstOrDefaultAsync(q => q.Id == id);
        if (area == null)
            throw new KeyNotFoundException("Area not found.");

        _context.Areas.Remove(area);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> IsAreaNameUniqueAsync(string name, int? excludeAreaId = null)
    {
        var query = _context.Areas
            .Where(a => a.Name == name);

        if (excludeAreaId.HasValue)
            query = query.Where(a => a.Id != excludeAreaId.Value);

        return !await query.AnyAsync();
    }

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