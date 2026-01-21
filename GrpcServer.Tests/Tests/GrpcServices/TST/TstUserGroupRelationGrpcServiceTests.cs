using Grpc.Core;
using Moq;
using GrpcServer.Infrastructure.GrpcServices.TST;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Protos.Common;

namespace GrpcServer.Tests.Tests.GrpcServices.TST;

/// <summary>
/// Unit tests for TstUserGroupRelationGrpcService covering all gRPC operations.
/// Tests mirror the controller layer scenarios using mocked dependencies.
/// </summary>
public class TstUserGroupRelationGrpcServiceTests
{
    private readonly Mock<IUserGroupRelationService<TstUser, TstGroup>> _mockRelationService;
    private readonly TstProtoMapper _mapper;
    private readonly TstUserGroupRelationGrpcService _grpcService;

    public TstUserGroupRelationGrpcServiceTests()
    {
        _mockRelationService = new Mock<IUserGroupRelationService<TstUser, TstGroup>>();
        _mapper = new TstProtoMapper();
        _grpcService = new TstUserGroupRelationGrpcService(_mockRelationService.Object, _mapper);
    }


    [Fact]
    public async Task GetUserGroups_WithExistingUserAndGroups_ReturnsGroups()
    {
        // Arrange
        var groups = new List<TstGroup>
        {
            new TstGroup
            {
                Id = "group1",
                DisplayName = "Engineering",
                TstGroupExtension1 = "ext1",
                TstGroupExtension2 = "ext2"
            },
            new TstGroup
            {
                Id = "group2",
                DisplayName = "Marketing",
                TstGroupExtension1 = "ext3",
                TstGroupExtension2 = "ext4"
            }
        };

        _mockRelationService.Setup(s => s.GetUserGroupsAsync("user1")).ReturnsAsync(groups);
        var request = new GetUserGroupsRequest { UserId = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetUserGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Groups.Count);
        Assert.Equal("group1", response.Groups[0].Base.Id);
        Assert.Equal("Engineering", response.Groups[0].Base.DisplayName);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task GetUserGroups_WithExistingUserNoGroups_ReturnsEmptyList()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetUserGroupsAsync("user1"))
            .ReturnsAsync(new List<TstGroup>());
        var request = new GetUserGroupsRequest { UserId = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetUserGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Groups);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task GetUserGroups_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetUserGroupsAsync("nonexistent"))
            .ThrowsAsync(new InvalidOperationException("User not found"));
        var request = new GetUserGroupsRequest { UserId = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetUserGroups(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync("nonexistent"), Times.Once);
    }

    [Fact]
    public async Task GetUserGroups_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetUserGroupsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));
        var request = new GetUserGroupsRequest { UserId = "user1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetUserGroups(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_WithValidRequest_AddsUserToGroups()
    {
        // Arrange
        var groupIds = new List<string> { "group1", "group2", "group3" };
        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("user1", It.IsAny<List<string>>()))
            .Returns(Task.CompletedTask);
        var request = new AddUserToGroupsRequest { UserId = "user1" };
        request.GroupIds.AddRange(groupIds);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUserToGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("3 group(s)", response.Message);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("user1", 
            It.Is<List<string>>(list => list.SequenceEqual(groupIds))), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_WithEmptyGroupIdsList_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new AddUserToGroupsRequest { UserId = "user1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUserToGroups(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("cannot be empty", exception.Status.Detail);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task AddUserToGroups_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        var groupIds = new List<string> { "group1", "group2" };
        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("nonexistent", It.IsAny<List<string>>()))
            .ThrowsAsync(new InvalidOperationException("User not found"));
        var request = new AddUserToGroupsRequest { UserId = "nonexistent" };
        request.GroupIds.AddRange(groupIds);
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUserToGroups(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("nonexistent", It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        var groupIds = new List<string> { "group1", "nonexistent" };
        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("user1", It.IsAny<List<string>>()))
            .ThrowsAsync(new InvalidOperationException("Group not found"));
        var request = new AddUserToGroupsRequest { UserId = "user1" };
        request.GroupIds.AddRange(groupIds);
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUserToGroups(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("user1", It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        var groupIds = new List<string> { "group1", "group2" };
        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("user1", It.IsAny<List<string>>()))
            .ThrowsAsync(new Exception("Database error"));
        var request = new AddUserToGroupsRequest { UserId = "user1" };
        request.GroupIds.AddRange(groupIds);
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUserToGroups(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("user1", It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithValidRequest_RemovesUserFromGroup()
    {
        // Arrange
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("user1", "group1"))
            .Returns(Task.CompletedTask);
        var request = new RemoveUserFromGroupRequest { UserId = "user1", GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.RemoveUserFromGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("removed from group", response.Message);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("user1", "group1"), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("nonexistent", "group1"))
            .ThrowsAsync(new InvalidOperationException("User not found"));
        var request = new RemoveUserFromGroupRequest { UserId = "nonexistent", GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.RemoveUserFromGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("nonexistent", "group1"), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("user1", "nonexistent"))
            .ThrowsAsync(new InvalidOperationException("Group not found"));
        var request = new RemoveUserFromGroupRequest { UserId = "user1", GroupId = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.RemoveUserFromGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("user1", "nonexistent"), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("user1", "group1"))
            .ThrowsAsync(new Exception("Database error"));
        var request = new RemoveUserFromGroupRequest { UserId = "user1", GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.RemoveUserFromGroup(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("user1", "group1"), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_WithExistingGroupAndUsers_ReturnsUsers()
    {
        // Arrange
        var users = new List<TstUser>
        {
            new TstUser
            {
                Id = "user1",
                UserName = "john.doe",
                Email = "john@example.com",
                TstUserExtension1 = "ext1",
                TstUserExtension2 = "ext2"
            },
            new TstUser
            {
                Id = "user2",
                UserName = "jane.doe",
                Email = "jane@example.com",
                TstUserExtension1 = "ext3",
                TstUserExtension2 = "ext4"
            }
        };

        _mockRelationService.Setup(s => s.GetGroupUsersAsync("group1")).ReturnsAsync(users);
        var request = new GetGroupUsersRequest { GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetGroupUsers(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Users.Count);
        Assert.Equal("user1", response.Users[0].Base.Id);
        Assert.Equal("john.doe", response.Users[0].Base.UserName);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_WithExistingGroupNoUsers_ReturnsEmptyList()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetGroupUsersAsync("group1"))
            .ReturnsAsync(new List<TstUser>());
        var request = new GetGroupUsersRequest { GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetGroupUsers(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Users);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetGroupUsersAsync("nonexistent"))
            .ThrowsAsync(new InvalidOperationException("Group not found"));
        var request = new GetGroupUsersRequest { GroupId = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetGroupUsers(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync("nonexistent"), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetGroupUsersAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));
        var request = new GetGroupUsersRequest { GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetGroupUsers(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_WithValidRequest_AddsUsersToGroup()
    {
        // Arrange
        var userIds = new List<string> { "user1", "user2", "user3" };
        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("group1", It.IsAny<List<string>>()))
            .Returns(Task.CompletedTask);
        var request = new AddUsersToGroupRequest { GroupId = "group1" };
        request.UserIds.AddRange(userIds);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUsersToGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("3 user(s)", response.Message);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("group1", 
            It.Is<List<string>>(list => list.SequenceEqual(userIds))), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_WithEmptyUserIdsList_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new AddUsersToGroupRequest { GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUsersToGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("cannot be empty", exception.Status.Detail);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task AddUsersToGroup_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        var userIds = new List<string> { "user1", "user2" };
        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("nonexistent", It.IsAny<List<string>>()))
            .ThrowsAsync(new InvalidOperationException("Group not found"));
        var request = new AddUsersToGroupRequest { GroupId = "nonexistent" };
        request.UserIds.AddRange(userIds);
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUsersToGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("nonexistent", It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        var userIds = new List<string> { "user1", "nonexistent" };
        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("group1", It.IsAny<List<string>>()))
            .ThrowsAsync(new InvalidOperationException("User not found"));
        var request = new AddUsersToGroupRequest { GroupId = "group1" };
        request.UserIds.AddRange(userIds);
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUsersToGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("group1", It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        var userIds = new List<string> { "user1", "user2" };
        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("group1", It.IsAny<List<string>>()))
            .ThrowsAsync(new Exception("Database error"));
        var request = new AddUsersToGroupRequest { GroupId = "group1" };
        request.UserIds.AddRange(userIds);
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUsersToGroup(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("group1", It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_WithSingleGroup_AddsUserToGroup()
    {
        // Arrange
        var groupIds = new List<string> { "group1" };
        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("user1", It.IsAny<List<string>>()))
            .Returns(Task.CompletedTask);
        var request = new AddUserToGroupsRequest { UserId = "user1" };
        request.GroupIds.AddRange(groupIds);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUserToGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("user1", response.Message);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("user1", It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_WithSingleUser_AddsUserToGroup()
    {
        // Arrange
        var userIds = new List<string> { "user1" };
        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("group1", It.IsAny<List<string>>()))
            .Returns(Task.CompletedTask);
        var request = new AddUsersToGroupRequest { GroupId = "group1" };
        request.UserIds.AddRange(userIds);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUsersToGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("group1", response.Message);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("group1", It.IsAny<List<string>>()), Times.Once);
    }
}

