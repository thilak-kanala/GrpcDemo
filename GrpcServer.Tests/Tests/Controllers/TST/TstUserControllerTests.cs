using Microsoft.AspNetCore.Mvc;
using Moq;
using GrpcServer.Infrastructure.Controllers.TST;
using GrpcServer.Infrastructure.DTO.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Mappers.TST;

namespace GrpcServer.Tests.Tests.Controllers.TST;

public class TstUserControllerTests
{
    private readonly Mock<IUserService<TstUser>> _mockUserService;
    private readonly TstMapper _mapper;
    private readonly TstUserController _controller;

    public TstUserControllerTests()
    {
        _mockUserService = new Mock<IUserService<TstUser>>();
        _mapper = new TstMapper();
        _controller = new TstUserController(_mockUserService.Object, _mapper);
    }

    [Fact]
    public async Task GetAllUsers_WithExistingUsers_ReturnsOkWithUsers()
    {
        // Arrange
        var users = new List<TstUser>
        {
            new TstUser
            {
                Id = "user1",
                UserName = "john.doe",
                Email = "john@example.com",
                TstUserExtension1 = "extension1",
                TstUserExtension2 = "extension2"
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

        _mockUserService.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<TstUserResponseDto>>(okResult.Value).ToList();
        Assert.Equal(2, returnedUsers.Count);
        Assert.Equal("user1", returnedUsers.First().Id);
        Assert.Equal("john.doe", returnedUsers.First().UserName);
        
        _mockUserService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_WithNoUsers_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<TstUser>());

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<TstUserResponseDto>>(okResult.Value);
        Assert.Empty(returnedUsers);
        
        _mockUserService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockUserService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WithExistingUser_ReturnsOkWithUser()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.GetByIdAsync("user1")).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById("user1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<TstUserResponseDto>(okResult.Value);
        Assert.Equal("user1", returnedUser.Id);
        Assert.Equal("john.doe", returnedUser.UserName);
        Assert.Equal("john@example.com", returnedUser.Email);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((TstUser?)null);

        // Act
        var result = await _controller.GetUserById("nonexistent");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockUserService.Verify(s => s.GetByIdAsync("nonexistent"), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetUserById("user1");

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithValidUser_ReturnsCreatedAtAction()
    {
        // Arrange
        var requestDto = new TstUserRequestDto(
            "user1",
            "john.doe",
            "john@example.com",
            "extension1",
            "extension2"
        );

        _mockUserService.Setup(s => s.AddAsync(It.IsAny<TstUser>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateUser(requestDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TstUserController.GetUserById), createdAtActionResult.ActionName);
        
        var returnedUser = Assert.IsType<TstUserResponseDto>(createdAtActionResult.Value);
        Assert.Equal("user1", returnedUser.Id);
        Assert.Equal("john.doe", returnedUser.UserName);
        
        _mockUserService.Verify(s => s.AddAsync(It.Is<TstUser>(u => 
            u.Id == "user1" && 
            u.UserName == "john.doe" && 
            u.Email == "john@example.com"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithInvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var requestDto = new TstUserRequestDto(
            "",
            "john.doe",
            "john@example.com",
            "extension1",
            "extension2"
        );

        _mockUserService.Setup(s => s.AddAsync(It.IsAny<TstUser>()))
            .ThrowsAsync(new ArgumentException("Id cannot be empty"));

        // Act
        var result = await _controller.CreateUser(requestDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequestResult.Value);
        
        _mockUserService.Verify(s => s.AddAsync(It.IsAny<TstUser>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var requestDto = new TstUserRequestDto(
            "user1",
            "john.doe",
            "john@example.com",
            "extension1",
            "extension2"
        );

        _mockUserService.Setup(s => s.AddAsync(It.IsAny<TstUser>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateUser(requestDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockUserService.Verify(s => s.AddAsync(It.IsAny<TstUser>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WithValidUserAndMatchingId_ReturnsNoContent()
    {
        // Arrange
        var requestDto = new TstUserRequestDto(
            "user1",
            "john.doe.updated",
            "john.updated@example.com",
            "extension1",
            "extension2"
        );

        var existingUser = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.GetByIdAsync("user1")).ReturnsAsync(existingUser);
        _mockUserService.Setup(s => s.UpdateAsync(It.IsAny<TstUser>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateUser("user1", requestDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
        _mockUserService.Verify(s => s.UpdateAsync(It.Is<TstUser>(u => 
            u.Id == "user1" && 
            u.UserName == "john.doe.updated" && 
            u.Email == "john.updated@example.com"
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var requestDto = new TstUserRequestDto(
            "user2",
            "john.doe",
            "john@example.com",
            "extension1",
            "extension2"
        );

        // Act
        var result = await _controller.UpdateUser("user1", requestDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        
        _mockUserService.Verify(s => s.GetByIdAsync(It.IsAny<string>()), Times.Never);
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<TstUser>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var requestDto = new TstUserRequestDto(
            "user1",
            "john.doe",
            "john@example.com",
            "extension1",
            "extension2"
        );

        _mockUserService.Setup(s => s.GetByIdAsync("user1")).ReturnsAsync((TstUser?)null);

        // Act
        var result = await _controller.UpdateUser("user1", requestDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<TstUser>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_WithValidationError_ReturnsBadRequest()
    {
        // Arrange
        var requestDto = new TstUserRequestDto(
            "user1",
            "",
            "john@example.com",
            "extension1",
            "extension2"
        );

        var existingUser = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.GetByIdAsync("user1")).ReturnsAsync(existingUser);
        _mockUserService.Setup(s => s.UpdateAsync(It.IsAny<TstUser>()))
            .ThrowsAsync(new ArgumentException("UserName cannot be empty"));

        // Act
        var result = await _controller.UpdateUser("user1", requestDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<TstUser>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var requestDto = new TstUserRequestDto(
            "user1",
            "john.doe",
            "john@example.com",
            "extension1",
            "extension2"
        );

        var existingUser = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.GetByIdAsync("user1")).ReturnsAsync(existingUser);
        _mockUserService.Setup(s => s.UpdateAsync(It.IsAny<TstUser>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateUser("user1", requestDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<TstUser>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WithExistingUser_ReturnsNoContent()
    {
        // Arrange
        var existingUser = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.GetByIdAsync("user1")).ReturnsAsync(existingUser);
        _mockUserService.Setup(s => s.DeleteAsync("user1")).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteUser("user1");

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
        _mockUserService.Verify(s => s.DeleteAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((TstUser?)null);

        // Act
        var result = await _controller.DeleteUser("nonexistent");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockUserService.Verify(s => s.GetByIdAsync("nonexistent"), Times.Once);
        _mockUserService.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var existingUser = new TstUser
        {
            Id = "user1",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.GetByIdAsync("user1")).ReturnsAsync(existingUser);
        _mockUserService.Setup(s => s.DeleteAsync("user1"))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteUser("user1");

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockUserService.Verify(s => s.DeleteAsync("user1"), Times.Once);
    }
}



