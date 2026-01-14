using Microsoft.AspNetCore.Mvc;
using Moq;
using GrpcServer.Infrastructure.Controllers.TST;
using GrpcServer.Infrastructure.DTO.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Mappers.TST;
using static GrpcServer.Infrastructure.DTO.Common.RelationDtos;

namespace GrpcServer.Tests.Tests.Controllers.TST;

public class TstUserGroupRelationControllerTests
{
    private readonly Mock<IUserGroupRelationService<TstUser, TstGroup>> _mockRelationService;
    private readonly TstMapper _mapper;
    private readonly TstUserGroupRelationController _controller;

    public TstUserGroupRelationControllerTests()
    {
        _mockRelationService = new Mock<IUserGroupRelationService<TstUser, TstGroup>>();
        _mapper = new TstMapper();
        _controller = new TstUserGroupRelationController(_mockRelationService.Object, _mapper);
    }

    [Fact]
    public async Task GetUserGroups_WithExistingUserAndGroups_ReturnsOkWithGroups()
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

        // Act
        var result = await _controller.GetUserGroups("user1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroups = Assert.IsAssignableFrom<IEnumerable<TstGroupResponseDto>>(okResult.Value).ToList();
        Assert.Equal(2, returnedGroups.Count);
        Assert.Equal("group1", returnedGroups.First().Id);
        Assert.Equal("Engineering", returnedGroups.First().DisplayName);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task GetUserGroups_WithExistingUserNoGroups_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetUserGroupsAsync("user1"))
            .ReturnsAsync(new List<TstGroup>());

        // Act
        var result = await _controller.GetUserGroups("user1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroups = Assert.IsAssignableFrom<IEnumerable<TstGroupResponseDto>>(okResult.Value);
        Assert.Empty(returnedGroups);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task GetUserGroups_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetUserGroupsAsync("nonexistent"))
            .ThrowsAsync(new InvalidOperationException("User not found"));

        // Act
        var result = await _controller.GetUserGroups("nonexistent");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync("nonexistent"), Times.Once);
    }

    [Fact]
    public async Task GetUserGroups_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetUserGroupsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetUserGroups("user1");

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new AddUserToGroupsRequestDto(new List<string> { "group1", "group2", "group3" });

        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("user1", request.GroupIds))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddUserToGroups("user1", request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("user1", request.GroupIds), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_WithEmptyGroupIdsList_ReturnsBadRequest()
    {
        // Arrange
        var request = new AddUserToGroupsRequestDto(new List<string>());

        // Act
        var result = await _controller.AddUserToGroups("user1", request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task AddUserToGroups_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var request = new AddUserToGroupsRequestDto(new List<string> { "group1", "group2" });

        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("nonexistent", request.GroupIds))
            .ThrowsAsync(new InvalidOperationException("User not found"));

        // Act
        var result = await _controller.AddUserToGroups("nonexistent", request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("nonexistent", request.GroupIds), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_WithNonExistentGroup_ReturnsNotFound()
    {
        // Arrange
        var request = new AddUserToGroupsRequestDto(new List<string> { "group1", "nonexistent" });

        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("user1", request.GroupIds))
            .ThrowsAsync(new InvalidOperationException("Group not found"));

        // Act
        var result = await _controller.AddUserToGroups("user1", request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("user1", request.GroupIds), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new AddUserToGroupsRequestDto(new List<string> { "group1", "group2" });

        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("user1", request.GroupIds))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.AddUserToGroups("user1", request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("user1", request.GroupIds), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("user1", "group1"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RemoveUserFromGroup("user1", "group1");

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("user1", "group1"), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("nonexistent", "group1"))
            .ThrowsAsync(new InvalidOperationException("User not found"));

        // Act
        var result = await _controller.RemoveUserFromGroup("nonexistent", "group1");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("nonexistent", "group1"), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_WithNonExistentGroup_ReturnsNotFound()
    {
        // Arrange
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("user1", "nonexistent"))
            .ThrowsAsync(new InvalidOperationException("Group not found"));

        // Act
        var result = await _controller.RemoveUserFromGroup("user1", "nonexistent");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("user1", "nonexistent"), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("user1", "group1"))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.RemoveUserFromGroup("user1", "group1");

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("user1", "group1"), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_WithExistingGroupAndUsers_ReturnsOkWithUsers()
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

        // Act
        var result = await _controller.GetGroupUsers("group1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<TstUserResponseDto>>(okResult.Value).ToList();
        Assert.Equal(2, returnedUsers.Count);
        Assert.Equal("user1", returnedUsers.First().Id);
        Assert.Equal("john.doe", returnedUsers.First().UserName);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_WithExistingGroupNoUsers_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetGroupUsersAsync("group1"))
            .ReturnsAsync(new List<TstUser>());

        // Act
        var result = await _controller.GetGroupUsers("group1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<TstUserResponseDto>>(okResult.Value);
        Assert.Empty(returnedUsers);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_WithNonExistentGroup_ReturnsNotFound()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetGroupUsersAsync("nonexistent"))
            .ThrowsAsync(new InvalidOperationException("Group not found"));

        // Act
        var result = await _controller.GetGroupUsers("nonexistent");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync("nonexistent"), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockRelationService.Setup(s => s.GetGroupUsersAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetGroupUsers("group1");

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new AddUsersToGroupRequestDto(new List<string> { "user1", "user2", "user3" });

        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("group1", request.UserIds))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddUsersToGroup("group1", request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("group1", request.UserIds), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_WithEmptyUserIdsList_ReturnsBadRequest()
    {
        // Arrange
        var request = new AddUsersToGroupRequestDto(new List<string>());

        // Act
        var result = await _controller.AddUsersToGroup("group1", request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task AddUsersToGroup_WithNonExistentGroup_ReturnsNotFound()
    {
        // Arrange
        var request = new AddUsersToGroupRequestDto(new List<string> { "user1", "user2" });

        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("nonexistent", request.UserIds))
            .ThrowsAsync(new InvalidOperationException("Group not found"));

        // Act
        var result = await _controller.AddUsersToGroup("nonexistent", request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("nonexistent", request.UserIds), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var request = new AddUsersToGroupRequestDto(new List<string> { "user1", "nonexistent" });

        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("group1", request.UserIds))
            .ThrowsAsync(new InvalidOperationException("User not found"));

        // Act
        var result = await _controller.AddUsersToGroup("group1", request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("group1", request.UserIds), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new AddUsersToGroupRequestDto(new List<string> { "user1", "user2" });

        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("group1", request.UserIds))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.AddUsersToGroup("group1", request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("group1", request.UserIds), Times.Once);
    }

    [Fact]
    public async Task AddUserToGroups_WithSingleGroup_ReturnsNoContent()
    {
        // Arrange
        var request = new AddUserToGroupsRequestDto(new List<string> { "group1" });

        _mockRelationService.Setup(s => s.AddUserToGroupsAsync("user1", request.GroupIds))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddUserToGroups("user1", request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockRelationService.Verify(s => s.AddUserToGroupsAsync("user1", request.GroupIds), Times.Once);
    }

    [Fact]
    public async Task AddUsersToGroup_WithSingleUser_ReturnsNoContent()
    {
        // Arrange
        var request = new AddUsersToGroupRequestDto(new List<string> { "user1" });

        _mockRelationService.Setup(s => s.AddUsersToGroupAsync("group1", request.UserIds))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddUsersToGroup("group1", request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockRelationService.Verify(s => s.AddUsersToGroupAsync("group1", request.UserIds), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromGroup_Idempotent_ReturnsNoContent()
    {
        // Arrange - removing a user that might not be in the group (idempotent operation)
        _mockRelationService.Setup(s => s.RemoveUserFromGroupAsync("user1", "group1"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RemoveUserFromGroup("user1", "group1");

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockRelationService.Verify(s => s.RemoveUserFromGroupAsync("user1", "group1"), Times.Once);
    }

    [Fact]
    public async Task GetUserGroups_WithSpecialCharactersInUserId_HandlesCorrectly()
    {
        // Arrange
        var userId = "user@domain.com";
        _mockRelationService.Setup(s => s.GetUserGroupsAsync(userId))
            .ReturnsAsync(new List<TstGroup>());

        // Act
        var result = await _controller.GetUserGroups(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroups = Assert.IsAssignableFrom<IEnumerable<TstGroupResponseDto>>(okResult.Value);
        Assert.Empty(returnedGroups);
        
        _mockRelationService.Verify(s => s.GetUserGroupsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetGroupUsers_WithSpecialCharactersInGroupId_HandlesCorrectly()
    {
        // Arrange
        var groupId = "group-with-dashes";
        _mockRelationService.Setup(s => s.GetGroupUsersAsync(groupId))
            .ReturnsAsync(new List<TstUser>());

        // Act
        var result = await _controller.GetGroupUsers(groupId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<TstUserResponseDto>>(okResult.Value);
        Assert.Empty(returnedUsers);
        
        _mockRelationService.Verify(s => s.GetGroupUsersAsync(groupId), Times.Once);
    }
}









