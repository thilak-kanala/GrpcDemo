using Grpc.Core;
using Moq;
using GrpcServer.Infrastructure.GrpcServices.TST;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Protos.Common;
using GrpcServer.Protos.TST;

namespace GrpcServer.Tests.Tests.GrpcServices.TST;

/// <summary>
/// Unit tests for TstUserGrpcService covering all gRPC operations.
/// Tests mirror the controller layer scenarios using mocked dependencies.
/// </summary>
public class TstUserGrpcServiceTests
{
    private readonly Mock<IUserService<TstUser>> _mockUserService;
    private readonly TstProtoMapper _mapper;
    private readonly TstUserGrpcService _grpcService;

    public TstUserGrpcServiceTests()
    {
        _mockUserService = new Mock<IUserService<TstUser>>();
        _mapper = new TstProtoMapper();
        _grpcService = new TstUserGrpcService(_mockUserService.Object, _mapper);
    }


    [Fact]
    public async Task GetAllUsers_WithExistingUsers_ReturnsUsers()
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
        var request = new GetAllUsersRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllUsers(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Users.Count);
        Assert.Equal("user1", response.Users[0].Base.Id);
        Assert.Equal("john.doe", response.Users[0].Base.UserName);
        
        _mockUserService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_WithNoUsers_ReturnsEmptyList()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<TstUser>());
        var request = new GetAllUsersRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllUsers(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Users);
        
        _mockUserService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("Database error"));
        var request = new GetAllUsersRequest();
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetAllUsers(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockUserService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WithExistingUser_ReturnsUser()
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
        var request = new GetUserByIdRequest { Id = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetUserById(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.User);
        Assert.Equal("user1", response.User.Base.Id);
        Assert.Equal("john.doe", response.User.Base.UserName);
        Assert.Equal("john@example.com", response.User.Base.Email);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((TstUser?)null);
        var request = new GetUserByIdRequest { Id = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetUserById(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
        
        _mockUserService.Verify(s => s.GetByIdAsync("nonexistent"), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));
        var request = new GetUserByIdRequest { Id = "user1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetUserById(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithValidUser_CreatesAndReturnsUser()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "john.doe",
                Email = "john@example.com"
            },
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.AddAsync(It.IsAny<TstUser>())).Returns(Task.CompletedTask);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateUser(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.User);
        Assert.Equal("user1", response.User.Base.Id);
        Assert.Equal("john.doe", response.User.Base.UserName);
        
        _mockUserService.Verify(s => s.AddAsync(It.Is<TstUser>(u => 
            u.Id == "user1" && 
            u.UserName == "john.doe" && 
            u.Email == "john@example.com"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithInvalidUser_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "",
                UserName = "john.doe",
                Email = "john@example.com"
            },
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.AddAsync(It.IsAny<TstUser>()))
            .ThrowsAsync(new ArgumentException("Id cannot be empty"));
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateUser(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        
        _mockUserService.Verify(s => s.AddAsync(It.IsAny<TstUser>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "john.doe",
                Email = "john@example.com"
            },
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.AddAsync(It.IsAny<TstUser>()))
            .ThrowsAsync(new Exception("Database error"));
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateUser(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockUserService.Verify(s => s.AddAsync(It.IsAny<TstUser>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WithValidUser_UpdatesUser()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "john.doe.updated",
                Email = "john.updated@example.com"
            },
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

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
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.UpdateUser(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("john.doe.updated", response.User.Base.UserName);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
        _mockUserService.Verify(s => s.UpdateAsync(It.Is<TstUser>(u => 
            u.Id == "user1" && 
            u.UserName == "john.doe.updated" && 
            u.Email == "john.updated@example.com"
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "john.doe",
                Email = "john@example.com"
            },
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

        _mockUserService.Setup(s => s.GetByIdAsync("user1")).ReturnsAsync((TstUser?)null);
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateUser(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<TstUser>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_WithValidationError_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "",
                Email = "john@example.com"
            },
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

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
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateUser(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<TstUser>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "john.doe",
                Email = "john@example.com"
            },
            TstUserExtension1 = "extension1",
            TstUserExtension2 = "extension2"
        };

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
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateUser(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockUserService.Verify(s => s.UpdateAsync(It.IsAny<TstUser>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WithExistingUser_DeletesUser()
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
        var request = new DeleteUserRequest { Id = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.DeleteUser(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("successfully deleted", response.Message);
        
        _mockUserService.Verify(s => s.GetByIdAsync("user1"), Times.Once);
        _mockUserService.Verify(s => s.DeleteAsync("user1"), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WithNonExistentUser_ThrowsNotFoundRpcException()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((TstUser?)null);
        var request = new DeleteUserRequest { Id = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.DeleteUser(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
        
        _mockUserService.Verify(s => s.GetByIdAsync("nonexistent"), Times.Once);
        _mockUserService.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_ServiceThrowsException_ThrowsRpcException()
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
        var request = new DeleteUserRequest { Id = "user1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.DeleteUser(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockUserService.Verify(s => s.DeleteAsync("user1"), Times.Once);
    }
}

