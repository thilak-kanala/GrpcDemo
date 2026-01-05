using GrpcServer.Tests.Infrastructure.Models.TST;
using GrpcServer.Tests.Infrastructure.Repositories.TST;
using Xunit;

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
        var tstGroup = Assert.IsType<TstGroup>(retrieved);
        Assert.Equal("group1", tstGroup.Id);
        Assert.Equal("Test Group", tstGroup.DisplayName);
        Assert.Equal("CustomGroupValue1", tstGroup.TstGroupExtension1);
        Assert.Equal("CustomGroupValue2", tstGroup.TstGroupExtension2);
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
        var tstGroup = Assert.IsType<TstGroup>(retrieved);
        Assert.Equal("Updated Group", tstGroup.DisplayName);
        Assert.Equal("UpdatedExt1", tstGroup.TstGroupExtension1);
        Assert.Equal("UpdatedExt2", tstGroup.TstGroupExtension2);
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
        var groups = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, groups.Count());
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
}

