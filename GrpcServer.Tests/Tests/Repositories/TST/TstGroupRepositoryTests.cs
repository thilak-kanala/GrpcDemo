using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Repositories.TST;

namespace GrpcServer.Tests.Tests.Repositories.TST;

public class TstGroupRepositoryTests
{
    [Fact]
    public async Task AddAsync_WithTstGroup_StoresTstSpecificProperties()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "CustomGroupValue1",
            TstGroupExtension2 = "CustomGroupValue2"
        };

        // Act
        await repository.AddAsync(group);
        var retrieved = await repository.GetByIdAsync("group1");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("group1", retrieved.Id);
        Assert.Equal("Test Group", retrieved.DisplayName);
        Assert.Equal("CustomGroupValue1", retrieved.TstGroupExtension1);
        Assert.Equal("CustomGroupValue2", retrieved.TstGroupExtension2);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateId_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var group1 = new TstGroup
        {
            Id = "group1",
            DisplayName = "First Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };
        var group2 = new TstGroup
        {
            Id = "group1",
            DisplayName = "Duplicate Group",
            TstGroupExtension1 = "Ext3",
            TstGroupExtension2 = "Ext4"
        };
        await repository.AddAsync(group1);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.AddAsync(group2));
        Assert.Contains("Group with ID 'group1' already exists", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithTstGroup_UpdatesTstSpecificProperties()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Original Group",
            TstGroupExtension1 = "OriginalExt1",
            TstGroupExtension2 = "OriginalExt2"
        };
        await repository.AddAsync(group);

        // Act
        var updatedGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "Updated Group",
            TstGroupExtension1 = "UpdatedExt1",
            TstGroupExtension2 = "UpdatedExt2"
        };
        await repository.UpdateAsync(updatedGroup);
        var retrieved = await repository.GetByIdAsync("group1");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Updated Group", retrieved.DisplayName);
        Assert.Equal("UpdatedExt1", retrieved.TstGroupExtension1);
        Assert.Equal("UpdatedExt2", retrieved.TstGroupExtension2);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var group = new TstGroup
        {
            Id = "nonexistent",
            DisplayName = "Non-existent Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.UpdateAsync(group));
        Assert.Contains("Group with ID 'nonexistent' not found", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var repository = new TstGroupRepository();

        // Act
        var result = await repository.GetByIdAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyRepository_ReturnsEmptyCollection()
    {
        // Arrange
        var repository = new TstGroupRepository();

        // Act
        var groups = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Empty(groups);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTstGroups()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var group1 = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };
        var group2 = new TstGroup
        {
            Id = "group2",
            DisplayName = "Group 2",
            TstGroupExtension1 = "Ext3",
            TstGroupExtension2 = "Ext4"
        };
        await repository.AddAsync(group1);
        await repository.AddAsync(group2);

        // Act
        var groups = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, groups.Count);
        Assert.All(groups, g => Assert.IsType<TstGroup>(g));
    }

    [Fact]
    public async Task DeleteAsync_RemovesGroup()
    {
        // Arrange
        var repository = new TstGroupRepository();
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };
        await repository.AddAsync(group);

        // Act
        await repository.DeleteAsync("group1");
        var retrieved = await repository.GetByIdAsync("group1");

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstGroupRepository();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.DeleteAsync("nonexistent"));
        Assert.Contains("Group with ID 'nonexistent' not found", exception.Message);
    }
}

