using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Repositories.TST;
using GrpcServer.Infrastructure.Services.TST;

namespace GrpcServer.Tests.Tests.Services.TST;

public class TstUserGroupRelationServiceTests
{
    [Fact]
    public async Task GetUserGroupsAsync_WithValidUser_ReturnsGroups()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        var group1 = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group One",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };
        var group2 = new TstGroup
        {
            Id = "group2",
            DisplayName = "Group Two",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };

        await userRepo.AddAsync(user);
        await groupRepo.AddAsync(group1);
        await groupRepo.AddAsync(group2);
        await relationRepo.AddUserToGroupAsync("user1", "group1");
        await relationRepo.AddUserToGroupAsync("user1", "group2");

        // Act
        var result = await service.GetUserGroupsAsync("user1");

        // Assert
        var groups = result.ToList();
        Assert.Equal(2, groups.Count);
    }

    [Fact]
    public async Task GetUserGroupsAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.GetUserGroupsAsync("nonexistent"));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task AddUserToGroupsAsync_WithValidData_CreatesRelations()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };

        await userRepo.AddAsync(user);
        await groupRepo.AddAsync(group);

        // Act
        await service.AddUserToGroupsAsync("user1", new List<string> { "group1" });

        // Assert
        var groupIds = await relationRepo.GetGroupIdsByUserIdAsync("user1");
        var groupIdsList = groupIds.ToList();
        Assert.Single(groupIdsList);
        Assert.Equal("group1", groupIdsList.First());
    }

    [Fact]
    public async Task AddUserToGroupsAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.AddUserToGroupsAsync("nonexistent", new List<string> { "group1" }));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task AddUserToGroupsAsync_WithNonExistentGroup_ThrowsInvalidOperationException()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        await userRepo.AddAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.AddUserToGroupsAsync("user1", new List<string> { "nonexistent" }));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_WithValidData_RemovesRelation()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };

        await userRepo.AddAsync(user);
        await groupRepo.AddAsync(group);
        await relationRepo.AddUserToGroupAsync("user1", "group1");

        // Act
        await service.RemoveUserFromGroupAsync("user1", "group1");

        // Assert
        var groupIds = await relationRepo.GetGroupIdsByUserIdAsync("user1");
        Assert.Empty(groupIds);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };
        await groupRepo.AddAsync(group);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.RemoveUserFromGroupAsync("nonexistent", "group1"));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task GetGroupUsersAsync_WithValidGroup_ReturnsUsers()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };
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

        await groupRepo.AddAsync(group);
        await userRepo.AddAsync(user1);
        await userRepo.AddAsync(user2);
        await relationRepo.AddUserToGroupAsync("user1", "group1");
        await relationRepo.AddUserToGroupAsync("user2", "group1");

        // Act
        var result = await service.GetGroupUsersAsync("group1");

        // Assert
        var users = result.ToList();
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task GetGroupUsersAsync_WithNonExistentGroup_ThrowsInvalidOperationException()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.GetGroupUsersAsync("nonexistent"));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task AddUsersToGroupAsync_WithValidData_CreatesRelations()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };
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

        await groupRepo.AddAsync(group);
        await userRepo.AddAsync(user1);
        await userRepo.AddAsync(user2);

        // Act
        await service.AddUsersToGroupAsync("group1", new List<string> { "user1", "user2" });

        // Assert
        var userIds = await relationRepo.GetUserIdsByGroupIdAsync("group1");
        Assert.Equal(2, userIds.Count());
    }

    [Fact]
    public async Task AddUsersToGroupAsync_WithNonExistentGroup_ThrowsInvalidOperationException()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var user = new TstUser
        {
            Id = "user1",
            UserName = "alice",
            Email = "alice@example.com",
            TstUserExtension1 = "Ext1",
            TstUserExtension2 = "Ext2"
        };
        await userRepo.AddAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.AddUsersToGroupAsync("nonexistent", new List<string> { "user1" }));
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task AddUsersToGroupAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var relationRepo = new TstUserGroupRelationRepository();
        var userRepo = new TstUserRepository();
        var groupRepo = new TstGroupRepository();
        var service = new TstUserGroupRelationService(relationRepo, userRepo, groupRepo);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Ext1",
            TstGroupExtension2 = "Ext2"
        };
        await groupRepo.AddAsync(group);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.AddUsersToGroupAsync("group1", new List<string> { "nonexistent" }));
        Assert.Contains("not found", exception.Message);
    }
}

