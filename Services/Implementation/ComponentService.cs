using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.Component;

namespace SGTD_WebApi.Services.Implementation;

public class ComponentService : IComponentService
{
    private readonly DatabaseContext _context;
    public ComponentService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(ComponentRequestParams requestParams)
    {
        var component = new Component
        {
            Name = requestParams.Name
        };
        _context.Components.Add(component);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ComponentRequestParams requestParams)
    {
        if (requestParams.Id == null)
            throw new ArgumentNullException(nameof(requestParams.Id), "Component Id is required for update.");
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == requestParams.Id);
        if (component == null)
            throw new KeyNotFoundException("Component not found.");
        component.Name = requestParams.Name;
        await _context.SaveChangesAsync();
    }

    public async Task<List<ComponentDto>> GetAllAsync()
    {
        return await _context.Components
            .Select(c => new ComponentDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<ComponentDto> GetByIdAsync(int id)
    {
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == id);
        if (component == null)
            throw new KeyNotFoundException("Component not found.");
        return new ComponentDto
        {
            Id = component.Id,
            Name = component.Name
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var component = await _context.Components.FirstOrDefaultAsync(c => c.Id == id);
        if (component == null)
            throw new KeyNotFoundException("Component not found.");
        _context.Components.Remove(component);
        await _context.SaveChangesAsync();
    }
}