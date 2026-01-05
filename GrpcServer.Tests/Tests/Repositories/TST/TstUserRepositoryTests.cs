using GrpcServer.Infrastructure.Models.Common;
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
    public async Task AddAsync_WithNonTstUser_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var mockUser = new MockUser { Id = "user1", UserName = "mock.user", Email = "mock@example.com" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await repository.AddAsync(mockUser));
        Assert.Contains("Only TstUser instances are supported", exception.Message);
        Assert.Equal("baseUser", exception.ParamName);
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
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.AddAsync(user));
        Assert.Contains("User with ID 'user1' already exists", exception.Message);
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
    public async Task UpdateAsync_WithNonTstUser_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var mockUser = new MockUser { Id = "user1", UserName = "mock.user", Email = "mock@example.com" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await repository.UpdateAsync(mockUser));
        Assert.Contains("Only TstUser instances are supported", exception.Message);
        Assert.Equal("baseUser", exception.ParamName);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var user = new TstUser
        {
            Id = "nonexistent",
            UserName = "nonexistent.user",
            Email = "nonexistent@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.UpdateAsync(user));
        Assert.Contains("User with ID 'nonexistent' not found", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var repository = new TstUserRepository();

        // Act
        var result = await repository.GetByIdAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyRepository_ReturnsEmptyCollection()
    {
        // Arrange
        var repository = new TstUserRepository();

        // Act
        var usersList = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Empty(usersList);
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
    public async Task DeleteAsync_RemovesUser()
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

        // Act
        await repository.DeleteAsync("user1");
        var retrieved = await repository.GetByIdAsync("user1");

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstUserRepository();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.DeleteAsync("nonexistent"));
        Assert.Contains("User with ID 'nonexistent' not found", exception.Message);
    }

    // Mock class for testing non-TstUser scenarios
    private class MockUser : IBaseUser
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }
}
