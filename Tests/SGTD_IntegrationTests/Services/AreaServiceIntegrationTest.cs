using Microsoft.EntityFrameworkCore;
using SGTD_IntegrationTests.Helpers;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.Models.Area;
using SGTD_WebApi.Services.Implementation;

namespace SGTD_IntegrationTests.Services;

public class AreaServiceIntegrationTest : IDisposable
{
    private readonly DatabaseContext _context;
    private readonly AreaService _areaService;

    public AreaServiceIntegrationTest()
    {
        _context = DatabaseContextFactory.CreateDbContext();
        var areaDependencyService = new AreaDependencyService(_context);
        _areaService = new AreaService(_context, areaDependencyService);
    }

    [Fact]
    public async Task CompleteAreaWorkflow_ShouldSucceed()
    {
        // 1. Create parent area
        var parentAreaRequest = new AreaRequestParams
        {
            Name = "Parent Area",
            Description = "Parent Description",
            Status = true
        };

        await _areaService.CreateAsync(parentAreaRequest);
        var parentArea = await _context.Areas.FirstAsync(a => a.Name == "Parent Area");
        Assert.NotNull(parentArea);

        // 2. Create child area with parent reference
        var childAreaRequest = new AreaRequestParams
        {
            Name = "Child Area",
            Description = "Child Description",
            Status = true,
            ParentAreaId = parentArea.Id
        };

        await _areaService.CreateAsync(childAreaRequest);

        // 3. Verify child area and its dependency
        var childArea = await _context.Areas.FirstAsync(a => a.Name == "Child Area");
        Assert.NotNull(childArea);

        var dependency = await _context.AreaDependencies
            .FirstOrDefaultAsync(ad => ad.ParentAreaId == parentArea.Id && ad.ChildAreaId == childArea.Id);
        Assert.NotNull(dependency);

        // 4. Update child area with new parent
        var newParentAreaRequest = new AreaRequestParams
        {
            Name = "New Parent Area",
            Description = "New Parent Description",
            Status = true
        };

        await _areaService.CreateAsync(newParentAreaRequest);
        var newParentArea = await _context.Areas.FirstAsync(a => a.Name == "New Parent Area");

        var updateChildRequest = new AreaRequestParams
        {
            Id = childArea.Id,
            Name = "Updated Child Area",
            Description = "Updated Child Description",
            Status = true,
            ParentAreaId = newParentArea.Id
        };

        await _areaService.UpdateAsync(updateChildRequest);

        // 5. Verify updated dependency
        var updatedDependency = await _context.AreaDependencies
            .FirstOrDefaultAsync(ad => ad.ParentAreaId == newParentArea.Id && ad.ChildAreaId == childArea.Id);
        Assert.NotNull(updatedDependency);

        var oldDependency = await _context.AreaDependencies
            .FirstOrDefaultAsync(ad => ad.ParentAreaId == parentArea.Id && ad.ChildAreaId == childArea.Id);
        Assert.Null(oldDependency);

        // 6. Get and verify all areas
        var allAreas = await _areaService.GetAllAsync();
        Assert.Equal(3, allAreas.Count);
        Assert.Contains(allAreas, a => a.Name == "New Parent Area");
        Assert.Contains(allAreas, a => a.Name == "Updated Child Area" && a.ParentAreaId == newParentArea.Id);
    }

    [Fact]
    public async Task AreaHierarchyManagement_ShouldHandleMultipleLevels()
    {
        // 1. Create root area
        var rootAreaRequest = new AreaRequestParams
        {
            Name = "Root Area",
            Description = "Root Level",
            Status = true
        };
        await _areaService.CreateAsync(rootAreaRequest);
        var rootArea = await _context.Areas.FirstAsync(a => a.Name == "Root Area");

        // 2. Create multiple child areas at different levels
        for (int i = 1; i <= 3; i++)
        {
            var childRequest = new AreaRequestParams
            {
                Name = $"Child Area {i}",
                Description = $"Level {i}",
                Status = true,
                ParentAreaId = rootArea.Id
            };
            await _areaService.CreateAsync(childRequest);
        }

        // 3. Verify hierarchy
        var allAreas = await _areaService.GetAllAsync();
        Assert.Equal(4, allAreas.Count);

        var childAreas = allAreas.Where(a => a.ParentAreaId == rootArea.Id);
        Assert.Equal(3, childAreas.Count());

        // 4. Add sub-child to first child
        var firstChild = await _context.Areas.FirstAsync(a => a.Name == "Child Area 1");
        var subChildRequest = new AreaRequestParams
        {
            Name = "Sub Child Area",
            Description = "Sub Level",
            Status = true,
            ParentAreaId = firstChild.Id
        };
        await _areaService.CreateAsync(subChildRequest);

        // 5. Verify updated hierarchy
        var updatedAreas = await _areaService.GetAllAsync();
        Assert.Equal(5, updatedAreas.Count);
        var subChild = updatedAreas.FirstOrDefault(a => a.Name == "Sub Child Area");
        Assert.NotNull(subChild);
        Assert.Equal(firstChild.Id, subChild.ParentAreaId);
    }

    [Fact]
    public async Task AreaDependencyConstraints_ShouldBeEnforced()
    {
        // 1. Create areas for testing constraints
        var area1Request = new AreaRequestParams { Name = "Area 1", Description = "Description 1" ,Status = true };
        var area2Request = new AreaRequestParams { Name = "Area 2", Description = "Description 2" ,Status = true };

        await _areaService.CreateAsync(area1Request);
        await _areaService.CreateAsync(area2Request);

        var area1 = await _context.Areas.FirstAsync(a => a.Name == "Area 1");
        var area2 = await _context.Areas.FirstAsync(a => a.Name == "Area 2");

        // 2. Create circular dependency (should fail)
        area2Request = new AreaRequestParams
        {
            Id = area2.Id,
            Name = "Area 2",
            Status = true,
            ParentAreaId = area1.Id
        };
        await _areaService.UpdateAsync(area2Request);

        // Attempting to create circular dependency
        area1Request = new AreaRequestParams
        {
            Id = area1.Id,
            Name = "Area 1",
            Status = true,
            ParentAreaId = area2.Id
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _areaService.UpdateAsync(area1Request));
    }

    [Fact]
    public async Task AreaStatusChanges_ShouldAffectVisibility()
    {
        // 1. Create active area
        var areaRequest = new AreaRequestParams
        {
            Name = "Test Area",
            Description = "Test Description",
            Status = true
        };
        await _areaService.CreateAsync(areaRequest);
        var area = await _context.Areas.FirstAsync(a => a.Name == "Test Area");

        // 2. Verify area is visible
        var activeAreas = await _areaService.GetAllAsync();
        Assert.Contains(activeAreas, a => a.Id == area.Id);

        // 3. Deactivate area
        var updateRequest = new AreaRequestParams
        {
            Id = area.Id,
            Name = "Test Area",
            Description = "Test Description",
            Status = false
        };
        await _areaService.UpdateAsync(updateRequest);

        // 4. Verify area status
        var updatedArea = await _areaService.GetByIdAsync(area.Id);
        Assert.False(updatedArea.Status);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}