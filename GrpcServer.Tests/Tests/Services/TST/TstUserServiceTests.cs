using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;
using GrpcServer.Tests.Infrastructure.Repositories.TST;
using GrpcServer.Tests.Infrastructure.Services.TST;
using GrpcServer.Tests.Infrastructure.Validators.TST;

namespace GrpcServer.Tests.Tests.Services.TST;

public class TstUserServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WithValidUser_ReturnsUser()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await repository.AddAsync(user);

        // Act
        var result = await service.GetByIdAsync("user1");

        // Assert
        Assert.NotNull(result);
        var tstUser = Assert.IsType<TstUser>(result);
        Assert.Equal("user1", tstUser.Id);
        Assert.Equal("john.doe", tstUser.UserName);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentUser_ReturnsNull()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);

        // Act
        var result = await service.GetByIdAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        
        var user1 = new TstUser
        {
            Id = "user1",
            UserName = "alice",
            Email = "alice@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        
        var user2 = new TstUser
        {
            Id = "user2",
            UserName = "bob",
            Email = "bob@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        
        await repository.AddAsync(user1);
        await repository.AddAsync(user2);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        var users = result.ToList();
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_NormalizesEmail()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "John@Example.COM",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };

        // Act
        await service.AddAsync(user);
        var result = await repository.GetByIdAsync("user1");

        // Assert
        Assert.NotNull(result);
        var tstUser = Assert.IsType<TstUser>(result);
        Assert.Equal("john@example.com", tstUser.Email); // Email normalized to lowercase
    }

    [Fact]
    public async Task AddAsync_WithNonTstUser_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var mockUser = new MockUser
        {
            Id = "user1",
            UserName = "mock.user",
            Email = "mock@example.com"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(mockUser));
        Assert.Contains("Only TstUser instances are supported", exception.Message);
    }

    [Fact]
    public async Task AddAsync_WithInvalidUser_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var user = new TstUser
        {
            Id = "user1",
            UserName = "", // Invalid: empty username
            Email = "john@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(user));
        Assert.Contains("validation failed", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithValidUser_NormalizesEmail()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
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
        var updatedUser = new TstUser
        {
            Id = "user1",
            UserName = "john.doe.updated",
            Email = "JOHN.Updated@Example.COM",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        await service.UpdateAsync(updatedUser);
        var result = await repository.GetByIdAsync("user1");

        // Assert
        Assert.NotNull(result);
        var tstUser = Assert.IsType<TstUser>(result);
        Assert.Equal("john.updated@example.com", tstUser.Email); // Email normalized
        Assert.Equal("john.doe.updated", tstUser.UserName);
    }

    [Fact]
    public async Task UpdateAsync_WithNonTstUser_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var mockUser = new MockUser
        {
            Id = "user1",
            UserName = "mock.user",
            Email = "mock@example.com"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(mockUser));
        Assert.Contains("Only TstUser instances are supported", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
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
        await service.DeleteAsync("user1");
        var result = await repository.GetByIdAsync("user1");

        // Assert
        Assert.Null(result); // User was deleted
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DeleteAsync("nonexistent"));
        Assert.Contains("not found", exception.Message);
    }

    // Tests for custom validation rules
    [Fact]
    public async Task AddAsync_WithForbiddenExtension1_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "forbidden", // Invalid: forbidden value
            TstUserExtension2 = "Ext2"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(user));
        Assert.Contains("validation failed", exception.Message);
    }

    [Fact]
    public async Task AddAsync_WithAllowedExtension1_Succeeds()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "allowed", // Valid: not forbidden
            TstUserExtension2 = "Ext2"
        };

        // Act
        await service.AddAsync(user);
        var result = await repository.GetByIdAsync("user1");

        // Assert
        Assert.NotNull(result);
        var tstUser = Assert.IsType<TstUser>(result);
        Assert.Equal("allowed", tstUser.TstUserExtension1);
    }

    [Fact]
    public async Task AddAsync_WithEmptyExtension1_Succeeds()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "", // Valid: empty is allowed (not forbidden)
            TstUserExtension2 = "Ext2"
        };

        // Act
        await service.AddAsync(user);
        var result = await repository.GetByIdAsync("user1");

        // Assert
        Assert.NotNull(result);
        var tstUser = Assert.IsType<TstUser>(result);
        Assert.Equal("", tstUser.TstUserExtension1);
    }

    [Fact]
    public async Task UpdateAsync_WithForbiddenExtension1_ThrowsArgumentException()
    {
        // Arrange
        var repository = new TstUserRepository();
        var validator = new TstUserValidator();
        var service = new TstUserService(repository, validator);
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "allowed",
            TstUserExtension2 = "Ext2"
        };
        await repository.AddAsync(user);

        // Act & Assert
        var updatedUser = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "forbidden", // Invalid: forbidden value
            TstUserExtension2 = "Ext2"
        };
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(updatedUser));
        Assert.Contains("validation failed", exception.Message);
    }
}

// Mock class for testing non-TstUser scenarios
internal class MockUser : IBaseUser
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
}

