using GrpcServer.Tests.Infrastructure.Models.TST;
using GrpcServer.Tests.Infrastructure.Repositories.TST;
using GrpcServer.Tests.Infrastructure.Services.TST;
using GrpcServer.Tests.Infrastructure.Validators.TST;

namespace GrpcServer.Tests.Tests.Services.TST;

public class TstGroupServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WithValidGroup_ReturnsGroup()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await repository.AddAsync(group);

        // Act
        var result = await service.GetByIdAsync("group1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("group1", result.Id);
        Assert.Equal("Test Group", result.DisplayName);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentGroup_ReturnsNull()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);

        // Act
        var result = await service.GetByIdAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGroups()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        
        var group1 = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group One",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Ext2"
        };
        
        var group2 = new TstGroup
        {
            Id = "group2",
            DisplayName = "Group Two",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Ext2"
        };
        
        await repository.AddAsync(group1);
        await repository.AddAsync(group2);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        var groups = result.ToList();
        Assert.Equal(2, groups.Count);
    }

    [Fact]
    public async Task AddAsync_WithValidGroup_TrimsDisplayName()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "  Test Group  ", // With whitespace
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Ext2"
        };

        // Act
        await service.AddAsync(group);
        var result = await repository.GetByIdAsync("group1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Group", result.DisplayName); // Whitespace trimmed
    }

    [Fact]
    public async Task AddAsync_WithInvalidGroup_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "", // Invalid: empty display name
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Ext2"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(group));
        Assert.Contains("validation failed", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithValidGroup_TrimsDisplayName()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Original Name",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Ext2"
        };
        await repository.AddAsync(group);

        // Act
        var updatedGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "  Updated Name  ",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Ext2"
        };
        await service.UpdateAsync(updatedGroup);
        var result = await repository.GetByIdAsync("group1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.DisplayName); // Trimmed
    }

    [Fact]
    public async Task DeleteAsync_RemovesGroup()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Ext2"
        };
        await repository.AddAsync(group);

        // Act
        await service.DeleteAsync("group1");
        var result = await repository.GetByIdAsync("group1");

        // Assert
        Assert.Null(result); // Group was deleted
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentGroup_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DeleteAsync("nonexistent"));
        Assert.Contains("not found", exception.Message);
    }

    // Tests for custom validation rules
    [Fact]
    public async Task AddAsync_WithExtension1LessThan5Chars_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext", // Invalid: less than 5 characters
            TstGroupExtension2 = "Ext2"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(group));
        Assert.Contains("validation failed", exception.Message);
    }

    [Fact]
    public async Task AddAsync_WithExtension1Exactly5Chars_Succeeds()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext12", // Valid: exactly 5 characters
            TstGroupExtension2 = "Ext2"
        };

        // Act
        await service.AddAsync(group);
        var result = await repository.GetByIdAsync("group1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Ext12", result.TstGroupExtension1);
    }

    [Fact]
    public async Task AddAsync_WithEmptyExtension1_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "", // Invalid: empty string
            TstGroupExtension2 = "Ext2"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(group));
        Assert.Contains("validation failed", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithExtension1LessThan5Chars_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        var service = new TstGroupService(repository, validator);
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "ValidExt",
            TstGroupExtension2 = "Ext2"
        };
        await repository.AddAsync(group);

        // Act & Assert
        var updatedGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Bad", // Invalid: less than 5 characters
            TstGroupExtension2 = "Ext2"
        };
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(updatedGroup));
        Assert.Contains("validation failed", exception.Message);
    }
}


