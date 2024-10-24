using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.AreaDependency;

namespace SGTD_WebApi.Services.Implementation;

public class AreaDependencyService : IAreaDependencyService
{
    private readonly DatabaseContext _context;

    public AreaDependencyService(DatabaseContext context)
    {
        _context = context;
    }

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

    public async Task DeleteByIdAsync(int id)
    {
        var areaDependency = await _context.AreaDependencies.FirstOrDefaultAsync(ad => ad.Id == id);
        if (areaDependency == null)
            throw new KeyNotFoundException("AreaDependency not found.");

        _context.AreaDependencies.Remove(areaDependency);
        await _context.SaveChangesAsync();
    }
}