
using Microsoft.EntityFrameworkCore;
using Moq;
using SGTD_UnitTests.Helpers;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.Area;
using SGTD_WebApi.Models.AreaDependency;
using SGTD_WebApi.Services;
using SGTD_WebApi.Services.Implementation;

namespace SGTD_UnitTests.Services;

public class AreaServiceUnitTest : IDisposable
{
    private readonly DatabaseContext _context;
    private readonly AreaService _areaService;
    private readonly Mock<IAreaDependencyService> _areaDependencyServiceMock;

    public AreaServiceUnitTest()
    {
        _context = DatabaseContextFactory.CreateDbContext();
        _areaDependencyServiceMock = new Mock<IAreaDependencyService>();
        _areaService = new AreaService(_context, _areaDependencyServiceMock.Object);
    }

    #region CreateAsync Tests
    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateArea()
    {
        // Arrange
        var areaRequest = new AreaRequestParams
        {
            Name = "Test Area",
            Description = "Test Description",
            Status = true
        };

        // Act
        await _areaService.CreateAsync(areaRequest);

        // Assert
        var area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == "Test Area");
        Assert.NotNull(area);
        Assert.Equal("Test Description", area.Description);
        Assert.True(area.Status);
    }

    [Fact]
    public async Task CreateAsync_WithParentArea_ShouldCreateAreaAndDependency()
    {
        // Arrange
        var areaRequest = new AreaRequestParams
        {
            Name = "Child Area",
            Description = "Child Description",
            Status = true,
            ParentAreaId = 1
        };

        _areaDependencyServiceMock.Setup(x => x.CreateAsync(It.IsAny<AreaDependencyRequestParams>()))
            .Returns(Task.CompletedTask);

        // Act
        await _areaService.CreateAsync(areaRequest);

        // Assert
        var area = await _context.Areas.FirstOrDefaultAsync(a => a.Name == "Child Area");
        Assert.NotNull(area);
        _areaDependencyServiceMock.Verify(x => x.CreateAsync(It.Is<AreaDependencyRequestParams>(
            p => p.ParentAreaId == 1 && p.ChildAreaId == area.Id)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var areaRequest = new AreaRequestParams
        {
            Name = "Test Area",
            Description = "Test Description",
            Status = true
        };

        await _areaService.CreateAsync(areaRequest);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _areaService.CreateAsync(areaRequest));
    }
    #endregion

    #region UpdateAsync Tests
    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateArea()
    {
        // Arrange
        var area = new Area
        {
            Name = "Original Name",
            Description = "Original Description",
            Status = true
        };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        var updateRequest = new AreaRequestParams
        {
            Id = area.Id,
            Name = "Updated Name",
            Description = "Updated Description",
            Status = false
        };

        // Act
        await _areaService.UpdateAsync(updateRequest);

        // Assert
        var updatedArea = await _context.Areas.FindAsync(area.Id);
        Assert.NotNull(updatedArea);
        Assert.Equal("Updated Name", updatedArea.Name);
        Assert.Equal("Updated Description", updatedArea.Description);
        Assert.False(updatedArea.Status);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowException()
    {
        // Arrange
        var updateRequest = new AreaRequestParams
        {
            Id = 999,
            Name = "Updated Name",
            Description = "Updated Description",
            Status = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _areaService.UpdateAsync(updateRequest));
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateName_ShouldThrowException()
    {
        // Arrange
        var area1 = new Area { Name = "Area 1", Description = "Description 1", Status = true };
        var area2 = new Area { Name = "Area 2", Description = "Description 2", Status = true };
        _context.Areas.AddRange(area1, area2);
        await _context.SaveChangesAsync();

        var updateRequest = new AreaRequestParams
        {
            Id = area2.Id,
            Name = "Area 1",
            Description = "Updated Description",
            Status = true
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _areaService.UpdateAsync(updateRequest));
    }

    [Fact]
    public async Task UpdateAsync_WithParentAreaChange_ShouldUpdateDependency()
    {
        // Arrange
        var areas = new List<Area>
        {
            new() { Name = "Area 1", Description = "Description 1", Status = true },
            new() { Name = "Area 2", Description = "Description 2", Status = false }
        };
        _context.Areas.AddRange(areas);
        await _context.SaveChangesAsync();

        var updateRequest = new AreaRequestParams
        {
            Id = areas[0].Id,
            Name = "Test Area",
            Description = "Updated Description",
            Status = true,
            ParentAreaId = 2
        };

        _areaDependencyServiceMock.Setup(x => x.CreateAsync(It.IsAny<AreaDependencyRequestParams>()))
            .Returns(Task.CompletedTask);

        // Act
        await _areaService.UpdateAsync(updateRequest);

        // Assert
        _areaDependencyServiceMock.Verify(x => x.CreateAsync(It.Is<AreaDependencyRequestParams>(
            p => p.ParentAreaId == 2 && p.ChildAreaId == areas[0].Id)), Times.Once);
    }
    #endregion

    #region GetAllAsync Tests
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAreas()
    {
        // Arrange
        var areas = new List<Area>
        {
            new() { Name = "Area 1", Description = "Description 1", Status = true },
            new() { Name = "Area 2", Description = "Description 2", Status = false }
        };
        _context.Areas.AddRange(areas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _areaService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, a => a.Name == "Area 1");
        Assert.Contains(result, a => a.Name == "Area 2");
    }

    [Fact]
    public async Task GetAllAsync_WithNoAreas_ShouldReturnEmptyList()
    {
        // Act
        var result = await _areaService.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }
    #endregion

    #region GetByIdAsync Tests
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnArea()
    {
        // Arrange
        var area = new Area
        {
            Name = "Test Area",
            Description = "Test Description",
            Status = true
        };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        // Act
        var result = await _areaService.GetByIdAsync(area.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(area.Id, result.Id);
        Assert.Equal("Test Area", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.True(result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _areaService.GetByIdAsync(999));
    }
    #endregion

    #region DeleteByIdAsync Tests
    [Fact]
    public async Task DeleteByIdAsync_WithValidId_ShouldDeleteArea()
    {
        // Arrange
        var area = new Area
        {
            Name = "Test Area",
            Description = "Test Description",
            Status = true
        };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        // Act
        await _areaService.DeleteByIdAsync(area.Id);

        // Assert
        var deletedArea = await _context.Areas.FindAsync(area.Id);
        Assert.Null(deletedArea);
    }

    [Fact]
    public async Task DeleteByIdAsync_WithInvalidId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _areaService.DeleteByIdAsync(999));
    }
    #endregion

    #region IsAreaNameUniqueAsync Tests
    [Fact]
    public async Task IsAreaNameUniqueAsync_WithUniqueName_ShouldReturnTrue()
    {
        // Arrange
        var area = new Area { Name = "Existing Area", Description = "Description", Status = true };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        // Act
        var result = await _areaService.IsAreaNameUniqueAsync("New Area");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsAreaNameUniqueAsync_WithDuplicateName_ShouldReturnFalse()
    {
        // Arrange
        var area = new Area { Name = "Existing Area", Description = "Description", Status = true };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        // Act
        var result = await _areaService.IsAreaNameUniqueAsync("Existing Area");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsAreaNameUniqueAsync_WithExcludedId_ShouldReturnTrue()
    {
        // Arrange
        var area = new Area { Name = "Existing Area", Description = "Description", Status = true };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();

        // Act
        var result = await _areaService.IsAreaNameUniqueAsync("Existing Area", area.Id);

        // Assert
        Assert.True(result);
    }
    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}