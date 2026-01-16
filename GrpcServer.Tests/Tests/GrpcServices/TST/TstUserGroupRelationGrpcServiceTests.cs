using Grpc.Core;
using GrpcServer.Infrastructure.GrpcServices.TST;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Repositories.TST;
using GrpcServer.Infrastructure.Services.TST;
using GrpcServer.Protos.Common;

namespace GrpcServer.Tests.Tests.GrpcServices.TST;

/// <summary>
/// Comprehensive unit tests for TstUserGroupRelationGrpcService covering all gRPC operations and scenarios.
/// Tests include: successful operations, error handling, validation, and edge cases.
/// </summary>
public class TstUserGroupRelationGrpcServiceTests
{
    private readonly TstUserGroupRelationGrpcService _grpcService;
    private readonly TstUserRepository _userRepository;
    private readonly TstGroupRepository _groupRepository;
    private readonly TstUserGroupRelationRepository _relationRepository;
    private readonly TstUserGroupRelationService _relationService;
    private readonly TstProtoMapper _mapper;

    public TstUserGroupRelationGrpcServiceTests()
    {
        _userRepository = new TstUserRepository();
        _groupRepository = new TstGroupRepository();
        _relationRepository = new TstUserGroupRelationRepository();
        _relationService = new TstUserGroupRelationService(
            _relationRepository,
            _userRepository,
            _groupRepository);
        _mapper = new TstProtoMapper();
        _grpcService = new TstUserGroupRelationGrpcService(_relationService, _mapper);
    }

    #region GetUserGroups Tests

    [Fact]
    public async Task GetUserGroups_WithUserInNoGroups_ReturnsEmptyResponse()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var request = new GetUserGroupsRequest { UserId = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetUserGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Groups);
    }

    [Fact]
    public async Task GetUserGroups_WithUserInMultipleGroups_ReturnsAllGroups()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group1 = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var group2 = new TstGroup
        {
            Id = "group2",
            DisplayName = "Group 2",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group1);
        await _groupRepository.AddAsync(group2);

        await _relationRepository.AddUserToGroupAsync("user1", "group1");
        await _relationRepository.AddUserToGroupAsync("user1", "group2");

        var request = new GetUserGroupsRequest { UserId = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetUserGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Groups.Count);
        Assert.Contains(response.Groups, g => g.Base.Id == "group1");
        Assert.Contains(response.Groups, g => g.Base.Id == "group2");
    }

    [Fact]
    public async Task GetUserGroups_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new GetUserGroupsRequest { UserId = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetUserGroups(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GetUserGroups_VerifiesCorrectGroupMapping()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "CustomExt1",
            TstGroupExtension2 = "CustomExt2"
        };
        await _groupRepository.AddAsync(group);
        await _relationRepository.AddUserToGroupAsync("user1", "group1");

        var request = new GetUserGroupsRequest { UserId = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetUserGroups(request, context);

        // Assert
        var returnedGroup = response.Groups.First();
        Assert.Equal("group1", returnedGroup.Base.Id);
        Assert.Equal("Test Group", returnedGroup.Base.DisplayName);
        Assert.Equal("CustomExt1", returnedGroup.TstGroupExtension1);
        Assert.Equal("CustomExt2", returnedGroup.TstGroupExtension2);
    }

    #endregion

    #region AddUserToGroups Tests

    [Fact]
    public async Task AddUserToGroups_WithValidData_AddsUserToGroups()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group1 = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var group2 = new TstGroup
        {
            Id = "group2",
            DisplayName = "Group 2",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group1);
        await _groupRepository.AddAsync(group2);

        var request = new AddUserToGroupsRequest
        {
            UserId = "user1",
            GroupIds = { "group1", "group2" }
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUserToGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("2 group(s)", response.Message);

        // Verify relationships were created
        var userGroups = await _relationService.GetUserGroupsAsync("user1");
        Assert.Equal(2, userGroups.Count());
    }

    [Fact]
    public async Task AddUserToGroups_WithEmptyGroupIds_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var request = new AddUserToGroupsRequest
        {
            UserId = "user1",
            GroupIds = { } // Empty list
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUserToGroups(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("cannot be empty", exception.Status.Detail);
    }

    [Fact]
    public async Task AddUserToGroups_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var request = new AddUserToGroupsRequest
        {
            UserId = "nonexistent",
            GroupIds = { "group1" }
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUserToGroups(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task AddUserToGroups_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var request = new AddUserToGroupsRequest
        {
            UserId = "user1",
            GroupIds = { "nonexistent" }
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUserToGroups(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task AddUserToGroups_WithSingleGroup_Succeeds()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var request = new AddUserToGroupsRequest
        {
            UserId = "user1",
            GroupIds = { "group1" }
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUserToGroups(request, context);

        // Assert
        Assert.True(response.Success);
        Assert.Contains("user1", response.Message);
    }

    #endregion

    #region RemoveUserFromGroup Tests

    [Fact]
    public async Task RemoveUserFromGroup_WithValidData_RemovesRelationship()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);
        await _relationRepository.AddUserToGroupAsync("user1", "group1");

        var request = new RemoveUserFromGroupRequest
        {
            UserId = "user1",
            GroupId = "group1"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.RemoveUserFromGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("removed from group", response.Message);

        // Verify relationship was removed
        var userGroups = await _relationService.GetUserGroupsAsync("user1");
        Assert.Empty(userGroups);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var request = new RemoveUserFromGroupRequest
        {
            UserId = "nonexistent",
            GroupId = "group1"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.RemoveUserFromGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var request = new RemoveUserFromGroupRequest
        {
            UserId = "user1",
            GroupId = "nonexistent"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.RemoveUserFromGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithNonExistentRelationship_ThrowsNotFoundRpcException()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);
        // Note: Not adding relationship

        var request = new RemoveUserFromGroupRequest
        {
            UserId = "user1",
            GroupId = "group1"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.RemoveUserFromGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromGroup_VerifiesMessageContent()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);
        await _relationRepository.AddUserToGroupAsync("user1", "group1");

        var request = new RemoveUserFromGroupRequest
        {
            UserId = "user1",
            GroupId = "group1"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.RemoveUserFromGroup(request, context);

        // Assert
        Assert.Contains("user1", response.Message);
        Assert.Contains("group1", response.Message);
    }

    #endregion

    #region GetGroupUsers Tests

    [Fact]
    public async Task GetGroupUsers_WithGroupHavingNoUsers_ReturnsEmptyResponse()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var request = new GetGroupUsersRequest { GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetGroupUsers(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Users);
    }

    [Fact]
    public async Task GetGroupUsers_WithGroupHavingMultipleUsers_ReturnsAllUsers()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var user1 = new TstUser
        {
            Id = "user1",
            UserName = "testuser1",
            Email = "test1@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var user2 = new TstUser
        {
            Id = "user2",
            UserName = "testuser2",
            Email = "test2@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user1);
        await _userRepository.AddAsync(user2);

        await _relationRepository.AddUserToGroupAsync("user1", "group1");
        await _relationRepository.AddUserToGroupAsync("user2", "group1");

        var request = new GetGroupUsersRequest { GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetGroupUsers(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Users.Count);
        Assert.Contains(response.Users, u => u.Base.Id == "user1");
        Assert.Contains(response.Users, u => u.Base.Id == "user2");
    }

    [Fact]
    public async Task GetGroupUsers_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new GetGroupUsersRequest { GroupId = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetGroupUsers(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GetGroupUsers_VerifiesCorrectUserMapping()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "CustomExt1",
            TstUserExtension2 = "CustomExt2"
        };
        await _userRepository.AddAsync(user);
        await _relationRepository.AddUserToGroupAsync("user1", "group1");

        var request = new GetGroupUsersRequest { GroupId = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetGroupUsers(request, context);

        // Assert
        var returnedUser = response.Users.First();
        Assert.Equal("user1", returnedUser.Base.Id);
        Assert.Equal("testuser", returnedUser.Base.UserName);
        Assert.Equal("test@example.com", returnedUser.Base.Email);
        Assert.Equal("CustomExt1", returnedUser.TstUserExtension1);
        Assert.Equal("CustomExt2", returnedUser.TstUserExtension2);
    }

    #endregion

    #region AddUsersToGroup Tests

    [Fact]
    public async Task AddUsersToGroup_WithValidData_AddsUsersToGroup()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var user1 = new TstUser
        {
            Id = "user1",
            UserName = "testuser1",
            Email = "test1@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var user2 = new TstUser
        {
            Id = "user2",
            UserName = "testuser2",
            Email = "test2@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user1);
        await _userRepository.AddAsync(user2);

        var request = new AddUsersToGroupRequest
        {
            GroupId = "group1",
            UserIds = { "user1", "user2" }
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUsersToGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("2 user(s)", response.Message);

        // Verify relationships were created
        var groupUsers = await _relationService.GetGroupUsersAsync("group1");
        Assert.Equal(2, groupUsers.Count());
    }

    [Fact]
    public async Task AddUsersToGroup_WithEmptyUserIds_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var request = new AddUsersToGroupRequest
        {
            GroupId = "group1",
            UserIds = { } // Empty list
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUsersToGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("cannot be empty", exception.Status.Detail);
    }

    [Fact]
    public async Task AddUsersToGroup_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var request = new AddUsersToGroupRequest
        {
            GroupId = "nonexistent",
            UserIds = { "user1" }
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUsersToGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task AddUsersToGroup_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var request = new AddUsersToGroupRequest
        {
            GroupId = "group1",
            UserIds = { "nonexistent" }
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.AddUsersToGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task AddUsersToGroup_WithSingleUser_Succeeds()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var request = new AddUsersToGroupRequest
        {
            GroupId = "group1",
            UserIds = { "user1" }
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUsersToGroup(request, context);

        // Assert
        Assert.True(response.Success);
        Assert.Contains("group1", response.Message);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public async Task GetUserGroups_AfterAddingAndRemovingGroup_ReturnsCorrectGroups()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group1 = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var group2 = new TstGroup
        {
            Id = "group2",
            DisplayName = "Group 2",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group1);
        await _groupRepository.AddAsync(group2);

        var context = TestServerCallContext.Create();

        // Act - Add to both groups
        await _grpcService.AddUserToGroups(
            new AddUserToGroupsRequest { UserId = "user1", GroupIds = { "group1", "group2" } },
            context);

        // Remove from one group
        await _grpcService.RemoveUserFromGroup(
            new RemoveUserFromGroupRequest { UserId = "user1", GroupId = "group1" },
            context);

        var response = await _grpcService.GetUserGroups(
            new GetUserGroupsRequest { UserId = "user1" },
            context);

        // Assert
        Assert.Single(response.Groups);
        Assert.Equal("group2", response.Groups.First().Base.Id);
    }

    [Fact]
    public async Task AddUserToGroups_WithLargeNumberOfGroups_Succeeds()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var groupIds = new List<string>();
        for (int i = 0; i < 50; i++)
        {
            var groupId = $"group{i}";
            groupIds.Add(groupId);
            await _groupRepository.AddAsync(new TstGroup
            {
                Id = groupId,
                DisplayName = $"Group {i}",
                TstGroupExtension1 = "Extension1",
                TstGroupExtension2 = "Extension2"
            });
        }

        var request = new AddUserToGroupsRequest
        {
            UserId = "user1"
        };
        request.GroupIds.AddRange(groupIds);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUserToGroups(request, context);

        // Assert
        Assert.True(response.Success);
        var userGroups = await _relationService.GetUserGroupsAsync("user1");
        Assert.Equal(50, userGroups.Count());
    }

    [Fact]
    public async Task AddUsersToGroup_WithLargeNumberOfUsers_Succeeds()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var userIds = new List<string>();
        for (int i = 0; i < 50; i++)
        {
            var userId = $"user{i}";
            userIds.Add(userId);
            await _userRepository.AddAsync(new TstUser
            {
                Id = userId,
                UserName = $"testuser{i}",
                Email = $"test{i}@example.com",
                TstUserExtension1 = "Extension1",
                TstUserExtension2 = "Extension2"
            });
        }

        var request = new AddUsersToGroupRequest
        {
            GroupId = "group1"
        };
        request.UserIds.AddRange(userIds);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.AddUsersToGroup(request, context);

        // Assert
        Assert.True(response.Success);
        var groupUsers = await _relationService.GetGroupUsersAsync("group1");
        Assert.Equal(50, groupUsers.Count());
    }

    [Fact]
    public async Task GetGroupUsers_AfterAddingAndRemovingUsers_ReturnsCorrectUsers()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var user1 = new TstUser
        {
            Id = "user1",
            UserName = "testuser1",
            Email = "test1@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var user2 = new TstUser
        {
            Id = "user2",
            UserName = "testuser2",
            Email = "test2@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user1);
        await _userRepository.AddAsync(user2);

        var context = TestServerCallContext.Create();

        // Act - Add both users
        await _grpcService.AddUsersToGroup(
            new AddUsersToGroupRequest { GroupId = "group1", UserIds = { "user1", "user2" } },
            context);

        // Remove one user
        await _grpcService.RemoveUserFromGroup(
            new RemoveUserFromGroupRequest { UserId = "user1", GroupId = "group1" },
            context);

        var response = await _grpcService.GetGroupUsers(
            new GetGroupUsersRequest { GroupId = "group1" },
            context);

        // Assert
        Assert.Single(response.Users);
        Assert.Equal("user2", response.Users.First().Base.Id);
    }

    [Fact]
    public async Task AddUserToGroups_WithDuplicateRelationship_DoesNotCreateDuplicate()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _userRepository.AddAsync(user);

        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _groupRepository.AddAsync(group);

        var request = new AddUserToGroupsRequest
        {
            UserId = "user1",
            GroupIds = { "group1" }
        };
        var context = TestServerCallContext.Create();

        // Act - Add twice
        await _grpcService.AddUserToGroups(request, context);
        await _grpcService.AddUserToGroups(request, context);

        var response = await _grpcService.GetUserGroups(
            new GetUserGroupsRequest { UserId = "user1" },
            context);

        // Assert - Should still have only one relationship
        Assert.Single(response.Groups);
    }

    #endregion
}

