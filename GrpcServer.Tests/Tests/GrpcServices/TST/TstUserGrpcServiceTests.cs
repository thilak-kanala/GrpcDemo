using Grpc.Core;
using GrpcServer.Infrastructure.GrpcServices.TST;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Repositories.TST;
using GrpcServer.Infrastructure.Services.TST;
using GrpcServer.Infrastructure.Validators.TST;
using GrpcServer.Protos.Common;
using GrpcServer.Protos.TST;

namespace GrpcServer.Tests.Tests.GrpcServices.TST;

/// <summary>
/// Comprehensive unit tests for TstUserGrpcService covering all gRPC operations and scenarios.
/// Tests include: successful operations, error handling, validation, and edge cases.
/// </summary>
public class TstUserGrpcServiceTests
{
    private readonly TstUserGrpcService _grpcService;
    private readonly TstUserRepository _repository;
    private readonly TstProtoMapper _mapper;
    private readonly TstUserService _userService;

    public TstUserGrpcServiceTests()
    {
        _repository = new TstUserRepository();
        var validator = new TstUserValidator();
        _userService = new Infrastructure.Services.TST.TstUserService(_repository, validator);
        _mapper = new TstProtoMapper();
        _grpcService = new TstUserGrpcService(_userService, _mapper);
    }

    #region GetAllUsers Tests

    [Fact]
    public async Task GetAllUsers_WithNoUsers_ReturnsEmptyResponse()
    {
        // Arrange
        var request = new GetAllUsersRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllUsers(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Users);
    }

    [Fact]
    public async Task GetAllUsers_WithMultipleUsers_ReturnsAllUsers()
    {
        // Arrange
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
        await _repository.AddAsync(user1);
        await _repository.AddAsync(user2);

        var request = new GetAllUsersRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllUsers(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Users.Count);
        Assert.Contains(response.Users, u => u.Base.Id == "user1");
        Assert.Contains(response.Users, u => u.Base.Id == "user2");
    }

    [Fact]
    public async Task GetAllUsers_VerifiesCorrectMapping()
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
        await _repository.AddAsync(user);

        var request = new GetAllUsersRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllUsers(request, context);

        // Assert
        var returnedUser = response.Users.First();
        Assert.Equal("user1", returnedUser.Base.Id);
        Assert.Equal("testuser", returnedUser.Base.UserName);
        Assert.Equal("test@example.com", returnedUser.Base.Email);
        Assert.Equal("Extension1", returnedUser.TstUserExtension1);
        Assert.Equal("Extension2", returnedUser.TstUserExtension2);
    }

    #endregion

    #region GetUserById Tests

    [Fact]
    public async Task GetUserById_WithValidId_ReturnsUser()
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
        await _repository.AddAsync(user);

        var request = new GetUserByIdRequest { Id = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetUserById(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.User);
        Assert.Equal("user1", response.User.Base.Id);
        Assert.Equal("testuser", response.User.Base.UserName);
    }

    [Fact]
    public async Task GetUserById_WithNonExistentId_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new GetUserByIdRequest { Id = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetUserById(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
    }

    [Fact]
    public async Task GetUserById_VerifiesAllFieldsMapped()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "testuser",
            Email = "test@example.com",
            TstUserExtension1 = "CustomExtension1",
            TstUserExtension2 = "CustomExtension2"
        };
        await _repository.AddAsync(user);

        var request = new GetUserByIdRequest { Id = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetUserById(request, context);

        // Assert
        Assert.Equal("CustomExtension1", response.User.TstUserExtension1);
        Assert.Equal("CustomExtension2", response.User.TstUserExtension2);
    }

    #endregion

    #region CreateUser Tests

    [Fact]
    public async Task CreateUser_WithValidRequest_CreatesAndReturnsUser()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "testuser",
                Email = "test@example.com"
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateUser(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.User);
        Assert.Equal("user1", response.User.Base.Id);
        
        // Verify user was actually created in repository
        var createdUser = await _repository.GetByIdAsync("user1");
        Assert.NotNull(createdUser);
        Assert.Equal("testuser", createdUser.UserName);
    }

    [Fact]
    public async Task CreateUser_WithInvalidEmail_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "testuser",
                Email = "invalid-email" // Invalid email format
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateUser(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("validation failed", exception.Status.Detail);
    }

    [Fact]
    public async Task CreateUser_WithEmptyUserName_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "", // Invalid: empty username
                Email = "test@example.com"
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateUser(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithEmptyExtension1_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "testuser",
                Email = "test@example.com"
            },
            TstUserExtension1 = "", // Invalid: empty extension
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateUser(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task CreateUser_TrimsWhitespaceFromUserName()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "  testuser  ", // Has whitespace
                Email = "test@example.com"
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateUser(request, context);

        // Assert
        var createdUser = await _repository.GetByIdAsync("user1");
        Assert.Equal("testuser", createdUser?.UserName); // Whitespace trimmed
    }

    #endregion

    #region UpdateUser Tests

    [Fact]
    public async Task UpdateUser_WithValidRequest_UpdatesAndReturnsUser()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "originalname",
            Email = "original@example.com",
            TstUserExtension1 = "OriginalExt1",
            TstUserExtension2 = "OriginalExt2"
        };
        await _repository.AddAsync(user);

        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "updatedname",
                Email = "updated@example.com"
            },
            TstUserExtension1 = "UpdatedExt1",
            TstUserExtension2 = "UpdatedExt2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.UpdateUser(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("updatedname", response.User.Base.UserName);
        
        var updatedUser = await _repository.GetByIdAsync("user1");
        Assert.Equal("updatedname", updatedUser?.UserName);
        Assert.Equal("updated@example.com", updatedUser?.Email);
    }

    [Fact]
    public async Task UpdateUser_WithNonExistentId_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "nonexistent",
                UserName = "testuser",
                Email = "test@example.com"
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateUser(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
    }

    [Fact]
    public async Task UpdateUser_WithInvalidEmail_ThrowsInvalidArgumentRpcException()
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
        await _repository.AddAsync(user);

        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "testuser",
                Email = "invalid-email" // Invalid format
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateUser(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_TrimsWhitespaceFromFields()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "originalname",
            Email = "original@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _repository.AddAsync(user);

        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "  updatedname  ",
                Email = "  updated@example.com  "
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        await _grpcService.UpdateUser(request, context);

        // Assert
        var updatedUser = await _repository.GetByIdAsync("user1");
        Assert.Equal("updatedname", updatedUser?.UserName);
        Assert.Equal("updated@example.com", updatedUser?.Email);
    }

    #endregion

    #region DeleteUser Tests

    [Fact]
    public async Task DeleteUser_WithValidId_DeletesUserSuccessfully()
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
        await _repository.AddAsync(user);

        var request = new DeleteUserRequest { Id = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.DeleteUser(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("successfully deleted", response.Message);
        
        var deletedUser = await _repository.GetByIdAsync("user1");
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUser_WithNonExistentId_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new DeleteUserRequest { Id = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.DeleteUser(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
    }

    [Fact]
    public async Task DeleteUser_VerifiesMessageContent()
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
        await _repository.AddAsync(user);

        var request = new DeleteUserRequest { Id = "user1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.DeleteUser(request, context);

        // Assert
        Assert.Contains("user1", response.Message);
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public async Task CreateUser_WithSpecialCharactersInUserName_Succeeds()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "test.user-123_name",
                Email = "test@example.com"
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateUser(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("test.user-123_name", response.User.Base.UserName);
    }

    [Fact]
    public async Task GetAllUsers_WithLargeNumberOfUsers_ReturnsAll()
    {
        // Arrange
        for (int i = 0; i < 100; i++)
        {
            await _repository.AddAsync(new TstUser
            {
                Id = $"user{i}",
                UserName = $"testuser{i}",
                Email = $"test{i}@example.com",
                TstUserExtension1 = "Extension1",
                TstUserExtension2 = "Extension2"
            });
        }

        var request = new GetAllUsersRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllUsers(request, context);

        // Assert
        Assert.Equal(100, response.Users.Count);
    }

    [Fact]
    public async Task UpdateUser_WithOnlyRequiredFieldsChanged_Succeeds()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user1",
            UserName = "originalname",
            Email = "original@example.com",
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        await _repository.AddAsync(user);

        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user1",
                UserName = "updatedname",
                Email = "original@example.com" // Keep same email
            },
            TstUserExtension1 = "Extension1",
            TstUserExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.UpdateUser(request, context);

        // Assert
        Assert.Equal("updatedname", response.User.Base.UserName);
        Assert.Equal("original@example.com", response.User.Base.Email);
    }

    #endregion
}

/// <summary>
/// Mock implementation of ServerCallContext for testing gRPC services.
/// Provides a minimal implementation required for unit testing.
/// </summary>
internal class TestServerCallContext : ServerCallContext
{
    private readonly Metadata _requestHeaders;
    private readonly CancellationToken _cancellationToken;
    private readonly Metadata _responseTrailers;
    private readonly AuthContext _authContext;
    private readonly Dictionary<object, object> _userState;
    private WriteOptions? _writeOptions;

    private TestServerCallContext()
    {
        _requestHeaders = new Metadata();
        _cancellationToken = CancellationToken.None;
        _responseTrailers = new Metadata();
        _authContext = new AuthContext(null, new Dictionary<string, List<AuthProperty>>());
        _userState = new Dictionary<object, object>();
    }

    protected override string MethodCore => "TestMethod";
    protected override string HostCore => "localhost";
    protected override string PeerCore => "127.0.0.1";
    protected override DateTime DeadlineCore => DateTime.MaxValue;
    protected override Metadata RequestHeadersCore => _requestHeaders;
    protected override CancellationToken CancellationTokenCore => _cancellationToken;
    protected override Metadata ResponseTrailersCore => _responseTrailers;
    protected override Status StatusCore { get; set; }
    protected override WriteOptions? WriteOptionsCore { get => _writeOptions; set => _writeOptions = value; }
    protected override AuthContext AuthContextCore => _authContext;
    protected override IDictionary<object, object> UserStateCore => _userState;

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
    {
        throw new NotImplementedException();
    }

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
    {
        return Task.CompletedTask;
    }

    public static TestServerCallContext Create()
    {
        return new TestServerCallContext();
    }
}

