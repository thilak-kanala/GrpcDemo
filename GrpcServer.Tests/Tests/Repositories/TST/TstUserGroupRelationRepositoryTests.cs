using GrpcServer.Tests.Infrastructure.Repositories.TST;

namespace GrpcServer.Tests.Tests.Repositories.TST;

public class TstUserGroupRelationRepositoryTests
{
    [Fact]
    public async Task AddUserToGroupAsync_CreatesRelation()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();

        // Act
        await repository.AddUserToGroupAsync("user1", "group1");
        var groupIds = (await repository.GetGroupIdsByUserIdAsync("user1")).ToList();
        var userIds = (await repository.GetUserIdsByGroupIdAsync("group1")).ToList();

        // Assert
        Assert.Contains("group1", groupIds);
        Assert.Contains("user1", userIds);
    }

    [Fact]
    public async Task AddUserToGroupAsync_WithDuplicateRelation_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();
        await repository.AddUserToGroupAsync("user1", "group1");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await repository.AddUserToGroupAsync("user1", "group1"));
        Assert.Contains("Relation between User 'user1' and Group 'group1' already exists", exception.Message);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_RemovesRelation()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();
        await repository.AddUserToGroupAsync("user1", "group1");

        // Act
        await repository.RemoveUserFromGroupAsync("user1", "group1");
        var groupIds = (await repository.GetGroupIdsByUserIdAsync("user1")).ToList();
        var userIds = (await repository.GetUserIdsByGroupIdAsync("group1")).ToList();

        // Assert
        Assert.Empty(groupIds);
        Assert.Empty(userIds);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_WithNonExistentRelation_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await repository.RemoveUserFromGroupAsync("user1", "group1"));
        Assert.Contains("Relation between User 'user1' and Group 'group1' not found", exception.Message);
    }

    [Fact]
    public async Task GetGroupIdsByUserIdAsync_WithNonExistentUser_ReturnsEmptyCollection()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();

        // Act
        var groupIds = (await repository.GetGroupIdsByUserIdAsync("nonexistent")).ToList();

        // Assert
        Assert.Empty(groupIds);
    }

    [Fact]
    public async Task GetGroupIdsByUserIdAsync_ReturnsAllGroupsForUser()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();
        await repository.AddUserToGroupAsync("user1", "group1");
        await repository.AddUserToGroupAsync("user1", "group2");
        await repository.AddUserToGroupAsync("user1", "group3");

        // Act
        var groupIds = (await repository.GetGroupIdsByUserIdAsync("user1")).ToList();

        // Assert
        Assert.Equal(3, groupIds.Count);
        Assert.Contains("group1", groupIds);
        Assert.Contains("group2", groupIds);
        Assert.Contains("group3", groupIds);
    }

    [Fact]
    public async Task GetUserIdsByGroupIdAsync_WithNonExistentGroup_ReturnsEmptyCollection()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();

        // Act
        var userIds = (await repository.GetUserIdsByGroupIdAsync("nonexistent")).ToList();

        // Assert
        Assert.Empty(userIds);
    }

    [Fact]
    public async Task GetUserIdsByGroupIdAsync_ReturnsAllUsersInGroup()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();
        await repository.AddUserToGroupAsync("user1", "group1");
        await repository.AddUserToGroupAsync("user2", "group1");
        await repository.AddUserToGroupAsync("user3", "group1");

        // Act
        var userIds = (await repository.GetUserIdsByGroupIdAsync("group1")).ToList();

        // Assert
        Assert.Equal(3, userIds.Count);
        Assert.Contains("user1", userIds);
        Assert.Contains("user2", userIds);
        Assert.Contains("user3", userIds);
    }

    [Fact]
    public async Task AddUserToGroupAsync_WithMultipleUsers_MaintainsSeparateRelations()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();

        // Act
        await repository.AddUserToGroupAsync("user1", "group1");
        await repository.AddUserToGroupAsync("user1", "group2");
        await repository.AddUserToGroupAsync("user2", "group1");
        await repository.AddUserToGroupAsync("user2", "group3");

        // Assert
        var user1Groups = (await repository.GetGroupIdsByUserIdAsync("user1")).ToList();
        var user2Groups = (await repository.GetGroupIdsByUserIdAsync("user2")).ToList();
        var group1Users = (await repository.GetUserIdsByGroupIdAsync("group1")).ToList();

        Assert.Equal(2, user1Groups.Count);
        Assert.Contains("group1", user1Groups);
        Assert.Contains("group2", user1Groups);

        Assert.Equal(2, user2Groups.Count);
        Assert.Contains("group1", user2Groups);
        Assert.Contains("group3", user2Groups);

        Assert.Equal(2, group1Users.Count);
        Assert.Contains("user1", group1Users);
        Assert.Contains("user2", group1Users);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_OnlyRemovesSpecificRelation()
    {
        // Arrange
        var repository = new TstUserGroupRelationRepository();
        await repository.AddUserToGroupAsync("user1", "group1");
        await repository.AddUserToGroupAsync("user1", "group2");
        await repository.AddUserToGroupAsync("user2", "group1");

        // Act
        await repository.RemoveUserFromGroupAsync("user1", "group1");

        // Assert
        var user1Groups = (await repository.GetGroupIdsByUserIdAsync("user1")).ToList();
        var group1Users = (await repository.GetUserIdsByGroupIdAsync("group1")).ToList();

        Assert.Single(user1Groups);
        Assert.Contains("group2", user1Groups);
        Assert.DoesNotContain("group1", user1Groups);

        Assert.Single(group1Users);
        Assert.Contains("user2", group1Users);
        Assert.DoesNotContain("user1", group1Users);
    }
}
