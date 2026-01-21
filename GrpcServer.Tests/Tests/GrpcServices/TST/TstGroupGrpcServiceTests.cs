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
/// Unit tests for TstGroupGrpcService covering all gRPC operations.
/// Tests mirror the controller layer scenarios using mocked dependencies.
/// </summary>
public class TstGroupGrpcServiceTests
{
    private readonly Mock<IGroupService<TstGroup>> _mockGroupService;
    private readonly TstProtoMapper _mapper;
    private readonly TstGroupGrpcService _grpcService;

    public TstGroupGrpcServiceTests()
    {
        _mockGroupService = new Mock<IGroupService<TstGroup>>();
        _mapper = new TstProtoMapper();
        _grpcService = new TstGroupGrpcService(_mockGroupService.Object, _mapper);
    }


    [Fact]
    public async Task GetAllGroups_WithExistingGroups_ReturnsGroups()
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

        _mockGroupService.Setup(s => s.GetAllAsync()).ReturnsAsync(groups);
        var request = new GetAllGroupsRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Groups.Count);
        Assert.Equal("group1", response.Groups[0].Base.Id);
        Assert.Equal("Engineering", response.Groups[0].Base.DisplayName);
        
        _mockGroupService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllGroups_WithNoGroups_ReturnsEmptyList()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<TstGroup>());
        var request = new GetAllGroupsRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Groups);
        
        _mockGroupService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllGroups_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("Database error"));
        var request = new GetAllGroupsRequest();
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetAllGroups(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockGroupService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetGroupById_WithExistingGroup_ReturnsGroup()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Engineering",
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync(group);
        var request = new GetGroupByIdRequest { Id = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetGroupById(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Group);
        Assert.Equal("group1", response.Group.Base.Id);
        Assert.Equal("Engineering", response.Group.Base.DisplayName);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task GetGroupById_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((TstGroup?)null);
        var request = new GetGroupByIdRequest { Id = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetGroupById(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("nonexistent"), Times.Once);
    }

    [Fact]
    public async Task GetGroupById_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));
        var request = new GetGroupByIdRequest { Id = "group1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetGroupById(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task CreateGroup_WithValidGroup_CreatesAndReturnsGroup()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Engineering"
            },
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.AddAsync(It.IsAny<TstGroup>())).Returns(Task.CompletedTask);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Group);
        Assert.Equal("group1", response.Group.Base.Id);
        Assert.Equal("Engineering", response.Group.Base.DisplayName);
        
        _mockGroupService.Verify(s => s.AddAsync(It.Is<TstGroup>(g => 
            g.Id == "group1" && 
            g.DisplayName == "Engineering"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateGroup_WithInvalidGroup_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "",
                DisplayName = "Engineering"
            },
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.AddAsync(It.IsAny<TstGroup>()))
            .ThrowsAsync(new ArgumentException("Id cannot be empty"));
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        
        _mockGroupService.Verify(s => s.AddAsync(It.IsAny<TstGroup>()), Times.Once);
    }

    [Fact]
    public async Task CreateGroup_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Engineering"
            },
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.AddAsync(It.IsAny<TstGroup>()))
            .ThrowsAsync(new Exception("Database error"));
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateGroup(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockGroupService.Verify(s => s.AddAsync(It.IsAny<TstGroup>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGroup_WithValidGroup_UpdatesGroup()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Engineering Updated"
            },
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        var existingGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "Engineering",
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync(existingGroup);
        _mockGroupService.Setup(s => s.UpdateAsync(It.IsAny<TstGroup>())).Returns(Task.CompletedTask);
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.UpdateGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Engineering Updated", response.Group.Base.DisplayName);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
        _mockGroupService.Verify(s => s.UpdateAsync(It.Is<TstGroup>(g => 
            g.Id == "group1" && 
            g.DisplayName == "Engineering Updated"
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateGroup_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Engineering"
            },
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync((TstGroup?)null);
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
        _mockGroupService.Verify(s => s.UpdateAsync(It.IsAny<TstGroup>()), Times.Never);
    }

    [Fact]
    public async Task UpdateGroup_WithValidationError_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = ""
            },
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        var existingGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "Engineering",
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync(existingGroup);
        _mockGroupService.Setup(s => s.UpdateAsync(It.IsAny<TstGroup>()))
            .ThrowsAsync(new ArgumentException("DisplayName cannot be empty"));
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        
        _mockGroupService.Verify(s => s.UpdateAsync(It.IsAny<TstGroup>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGroup_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Engineering"
            },
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        var existingGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "Engineering",
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync(existingGroup);
        _mockGroupService.Setup(s => s.UpdateAsync(It.IsAny<TstGroup>()))
            .ThrowsAsync(new Exception("Database error"));
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateGroup(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockGroupService.Verify(s => s.UpdateAsync(It.IsAny<TstGroup>()), Times.Once);
    }

    [Fact]
    public async Task DeleteGroup_WithExistingGroup_DeletesGroup()
    {
        // Arrange
        var existingGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "Engineering",
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync(existingGroup);
        _mockGroupService.Setup(s => s.DeleteAsync("group1")).Returns(Task.CompletedTask);
        var request = new DeleteGroupRequest { Id = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.DeleteGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("successfully deleted", response.Message);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
        _mockGroupService.Verify(s => s.DeleteAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task DeleteGroup_WithNonExistentGroup_ThrowsNotFoundRpcException()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((TstGroup?)null);
        var request = new DeleteGroupRequest { Id = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.DeleteGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("nonexistent"), Times.Once);
        _mockGroupService.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteGroup_ServiceThrowsException_ThrowsRpcException()
    {
        // Arrange
        var existingGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "Engineering",
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync(existingGroup);
        _mockGroupService.Setup(s => s.DeleteAsync("group1"))
            .ThrowsAsync(new Exception("Database error"));
        var request = new DeleteGroupRequest { Id = "group1" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.DeleteGroup(request, context));
        
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
        
        _mockGroupService.Verify(s => s.DeleteAsync("group1"), Times.Once);
    }
}

