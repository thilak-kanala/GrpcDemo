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
/// Comprehensive unit tests for TstGroupGrpcService covering all gRPC operations and scenarios.
/// Tests include: successful operations, error handling, validation, and edge cases.
/// </summary>
public class TstGroupGrpcServiceTests
{
    private readonly TstGroupGrpcService _grpcService;
    private readonly TstGroupRepository _repository;
    private readonly TstProtoMapper _mapper;
    private readonly TstGroupService _groupService;

    public TstGroupGrpcServiceTests()
    {
        _repository = new TstGroupRepository();
        var validator = new TstGroupValidator();
        _groupService = new Infrastructure.Services.TST.TstGroupService(_repository, validator);
        _mapper = new TstProtoMapper();
        _grpcService = new TstGroupGrpcService(_groupService, _mapper);
    }

    #region GetAllGroups Tests

    [Fact]
    public async Task GetAllGroups_WithNoGroups_ReturnsEmptyResponse()
    {
        // Arrange
        var request = new GetAllGroupsRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Groups);
    }

    [Fact]
    public async Task GetAllGroups_WithMultipleGroups_ReturnsAllGroups()
    {
        // Arrange
        var group1 = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group 1",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var group2 = new TstGroup
        {
            Id = "group2",
            DisplayName = "Test Group 2",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _repository.AddAsync(group1);
        await _repository.AddAsync(group2);

        var request = new GetAllGroupsRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllGroups(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Groups.Count);
        Assert.Contains(response.Groups, g => g.Base.Id == "group1");
        Assert.Contains(response.Groups, g => g.Base.Id == "group2");
    }

    [Fact]
    public async Task GetAllGroups_VerifiesCorrectMapping()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "CustomExtension1",
            TstGroupExtension2 = "CustomExtension2"
        };
        await _repository.AddAsync(group);

        var request = new GetAllGroupsRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllGroups(request, context);

        // Assert
        var returnedGroup = response.Groups.First();
        Assert.Equal("group1", returnedGroup.Base.Id);
        Assert.Equal("Test Group", returnedGroup.Base.DisplayName);
        Assert.Equal("CustomExtension1", returnedGroup.TstGroupExtension1);
        Assert.Equal("CustomExtension2", returnedGroup.TstGroupExtension2);
    }

    #endregion

    #region GetGroupById Tests

    [Fact]
    public async Task GetGroupById_WithValidId_ReturnsGroup()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _repository.AddAsync(group);

        var request = new GetGroupByIdRequest { Id = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetGroupById(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Group);
        Assert.Equal("group1", response.Group.Base.Id);
        Assert.Equal("Test Group", response.Group.Base.DisplayName);
    }

    [Fact]
    public async Task GetGroupById_WithNonExistentId_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new GetGroupByIdRequest { Id = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.GetGroupById(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
    }

    [Fact]
    public async Task GetGroupById_VerifiesAllFieldsMapped()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "SpecialExt1",
            TstGroupExtension2 = "SpecialExt2"
        };
        await _repository.AddAsync(group);

        var request = new GetGroupByIdRequest { Id = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetGroupById(request, context);

        // Assert
        Assert.Equal("SpecialExt1", response.Group.TstGroupExtension1);
        Assert.Equal("SpecialExt2", response.Group.TstGroupExtension2);
    }

    #endregion

    #region CreateGroup Tests

    [Fact]
    public async Task CreateGroup_WithValidRequest_CreatesAndReturnsGroup()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Test Group"
            },
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Group);
        Assert.Equal("group1", response.Group.Base.Id);
        
        // Verify group was actually created in repository
        var createdGroup = await _repository.GetByIdAsync("group1");
        Assert.NotNull(createdGroup);
        Assert.Equal("Test Group", createdGroup.DisplayName);
    }

    [Fact]
    public async Task CreateGroup_WithEmptyDisplayName_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "" // Invalid: empty display name
            },
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains("validation failed", exception.Status.Detail);
    }

    [Fact]
    public async Task CreateGroup_WithExtension1LessThan5Chars_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Test Group"
            },
            TstGroupExtension1 = "Ext", // Invalid: less than 5 characters
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_WithEmptyExtension1_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Test Group"
            },
            TstGroupExtension1 = "", // Invalid: empty extension
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.CreateGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_TrimsWhitespaceFromDisplayName()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "  Test Group  " // Has whitespace
            },
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateGroup(request, context);

        // Assert
        var createdGroup = await _repository.GetByIdAsync("group1");
        Assert.Equal("Test Group", createdGroup?.DisplayName); // Whitespace trimmed
    }

    [Fact]
    public async Task CreateGroup_WithExtension1Exactly5Chars_Succeeds()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Test Group"
            },
            TstGroupExtension1 = "Ext12", // Valid: exactly 5 characters
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Ext12", response.Group.TstGroupExtension1);
    }

    #endregion

    #region UpdateGroup Tests

    [Fact]
    public async Task UpdateGroup_WithValidRequest_UpdatesAndReturnsGroup()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Original Name",
            TstGroupExtension1 = "OriginalExt1",
            TstGroupExtension2 = "OriginalExt2"
        };
        await _repository.AddAsync(group);

        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Updated Name"
            },
            TstGroupExtension1 = "UpdatedExt1",
            TstGroupExtension2 = "UpdatedExt2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.UpdateGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Updated Name", response.Group.Base.DisplayName);
        
        var updatedGroup = await _repository.GetByIdAsync("group1");
        Assert.Equal("Updated Name", updatedGroup?.DisplayName);
        Assert.Equal("UpdatedExt1", updatedGroup?.TstGroupExtension1);
    }

    [Fact]
    public async Task UpdateGroup_WithNonExistentId_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "nonexistent",
                DisplayName = "Test Group"
            },
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
    }

    [Fact]
    public async Task UpdateGroup_WithEmptyDisplayName_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _repository.AddAsync(group);

        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "" // Invalid: empty display name
            },
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateGroup_WithExtension1LessThan5Chars_ThrowsInvalidArgumentRpcException()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "ValidExt1",
            TstGroupExtension2 = "Extension2"
        };
        await _repository.AddAsync(group);

        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Test Group"
            },
            TstGroupExtension1 = "Bad", // Invalid: less than 5 characters
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.UpdateGroup(request, context));
        
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateGroup_TrimsWhitespaceFromFields()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Original Name",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _repository.AddAsync(group);

        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "  Updated Name  " // Has whitespace
            },
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        await _grpcService.UpdateGroup(request, context);

        // Assert
        var updatedGroup = await _repository.GetByIdAsync("group1");
        Assert.Equal("Updated Name", updatedGroup?.DisplayName);
    }

    #endregion

    #region DeleteGroup Tests

    [Fact]
    public async Task DeleteGroup_WithValidId_DeletesGroupSuccessfully()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _repository.AddAsync(group);

        var request = new DeleteGroupRequest { Id = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.DeleteGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Contains("successfully deleted", response.Message);
        
        var deletedGroup = await _repository.GetByIdAsync("group1");
        Assert.Null(deletedGroup);
    }

    [Fact]
    public async Task DeleteGroup_WithNonExistentId_ThrowsNotFoundRpcException()
    {
        // Arrange
        var request = new DeleteGroupRequest { Id = "nonexistent" };
        var context = TestServerCallContext.Create();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            async () => await _grpcService.DeleteGroup(request, context));
        
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        Assert.Contains("not found", exception.Status.Detail);
    }

    [Fact]
    public async Task DeleteGroup_VerifiesMessageContent()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Test Group",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _repository.AddAsync(group);

        var request = new DeleteGroupRequest { Id = "group1" };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.DeleteGroup(request, context);

        // Assert
        Assert.Contains("group1", response.Message);
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public async Task CreateGroup_WithSpecialCharactersInDisplayName_Succeeds()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Test-Group_123 & Co."
            },
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Test-Group_123 & Co.", response.Group.Base.DisplayName);
    }

    [Fact]
    public async Task GetAllGroups_WithLargeNumberOfGroups_ReturnsAll()
    {
        // Arrange
        for (int i = 0; i < 100; i++)
        {
            await _repository.AddAsync(new TstGroup
            {
                Id = $"group{i}",
                DisplayName = $"Test Group {i}",
                TstGroupExtension1 = "Extension1",
                TstGroupExtension2 = "Extension2"
            });
        }

        var request = new GetAllGroupsRequest();
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.GetAllGroups(request, context);

        // Assert
        Assert.Equal(100, response.Groups.Count);
    }

    [Fact]
    public async Task UpdateGroup_WithOnlyDisplayNameChanged_Succeeds()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group1",
            DisplayName = "Original Name",
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        await _repository.AddAsync(group);

        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Updated Name"
            },
            TstGroupExtension1 = "Extension1", // Keep same
            TstGroupExtension2 = "Extension2"  // Keep same
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.UpdateGroup(request, context);

        // Assert
        Assert.Equal("Updated Name", response.Group.Base.DisplayName);
        Assert.Equal("Extension1", response.Group.TstGroupExtension1);
        Assert.Equal("Extension2", response.Group.TstGroupExtension2);
    }

    [Fact]
    public async Task CreateGroup_WithLongDisplayName_Succeeds()
    {
        // Arrange
        var longName = new string('A', 200);
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = longName
            },
            TstGroupExtension1 = "Extension1",
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateGroup(request, context);

        // Assert
        Assert.Equal(longName, response.Group.Base.DisplayName);
    }

    [Fact]
    public async Task CreateGroup_WithExtension1AtMinimumLength_Succeeds()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group1",
                DisplayName = "Test Group"
            },
            TstGroupExtension1 = "12345", // Exactly 5 characters (minimum)
            TstGroupExtension2 = "Extension2"
        };
        var context = TestServerCallContext.Create();

        // Act
        var response = await _grpcService.CreateGroup(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("12345", response.Group.TstGroupExtension1);
    }

    #endregion
}

