using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.PositionDependency;

namespace SGTD_WebApi.Services.Implementation;

public class PositionDependencyService : IPositionDependencyService
{
    private readonly DatabaseContext _context;

    public PositionDependencyService(DatabaseContext context)
    {
        _context = context;
    }

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

    public async Task DeleteByIdAsync(int id)
    {
        var areaDependency = await _context.PositionsDependency.FirstOrDefaultAsync(ad => ad.Id == id);
        if (areaDependency == null)
            throw new KeyNotFoundException("PositionDependency not found.");

        _context.PositionsDependency.Remove(areaDependency);
        await _context.SaveChangesAsync();
    }
}