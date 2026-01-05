using GrpcServer.Tests.Infrastructure.Repositories.TST;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Tests.Repositories.TST;

public class TstUserRepositoryTests
{
    [Fact]
    public async Task AddAsync_WithTstUser_StoresTstSpecificProperties()
    {
        // Arrange
        var repository = new TstUserRepository();
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "CustomValue1",
            TstUserExtension2 = "CustomValue2"
        };

        // Act
        await repository.AddAsync(user);
        var retrieved = await repository.GetByIdAsync("user1");

        // Assert
        Assert.NotNull(retrieved);
        var tstUser = Assert.IsType<TstUser>(retrieved);
        Assert.Equal("user1", tstUser.Id);
        Assert.Equal("john.doe", tstUser.UserName);
        Assert.Equal("john@example.com", tstUser.Email);
        Assert.Equal("CustomValue1", tstUser.TstUserExtension1);
        Assert.Equal("CustomValue2", tstUser.TstUserExtension2);
    }

    [Fact]
    public async Task UpdateAsync_WithTstUser_UpdatesTstSpecificProperties()
    {
        // Arrange
        var repository = new TstUserRepository();
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "OriginalValue1",
            TstUserExtension2 = "OriginalValue2"
        };
        await repository.AddAsync(user);

        // Act
        var updatedUser = new TstUser
        {
            Id = "user1",
            UserName = "john.doe.updated",
            Email = "john.updated@example.com",
            TstUserExtension1 = "UpdatedValue1",
            TstUserExtension2 = "UpdatedValue2"
        };
        await repository.UpdateAsync(updatedUser);
        var retrieved = await repository.GetByIdAsync("user1");

        // Assert
        Assert.NotNull(retrieved);
        var tstUser = Assert.IsType<TstUser>(retrieved);
        Assert.Equal("john.doe.updated", tstUser.UserName);
        Assert.Equal("john.updated@example.com", tstUser.Email);
        Assert.Equal("UpdatedValue1", tstUser.TstUserExtension1);
        Assert.Equal("UpdatedValue2", tstUser.TstUserExtension2);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTstUsers()
    {
        // Arrange
        var repository = new TstUserRepository();
        var user1 = new TstUser
        {
            Id = "user1",
            UserName = "john",
            Email = "john@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        var user2 = new TstUser
        {
            Id = "user2",
            UserName = "jane",
            Email = "jane@example.com",
            TstUserExtension1 = "Ext3",
            TstUserExtension2 = "Ext4"
        };
        await repository.AddAsync(user1);
        await repository.AddAsync(user2);

        // Act
        var usersList = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, usersList.Count);
        Assert.All(usersList, u => Assert.IsType<TstUser>(u));
    }

    [Fact]
    public async Task AddAsync_ThrowsException_WhenUserAlreadyExists()
    {
        // Arrange
        var repository = new TstUserRepository();
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        await repository.AddAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.AddAsync(user));
    }
}
