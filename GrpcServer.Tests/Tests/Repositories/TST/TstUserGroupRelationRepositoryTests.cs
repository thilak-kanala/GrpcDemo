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
}
