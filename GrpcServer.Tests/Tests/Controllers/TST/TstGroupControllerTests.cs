using Microsoft.AspNetCore.Mvc;
using Moq;
using GrpcServer.Infrastructure.Controllers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Models.TST.DTO;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Mappers.TST;

namespace GrpcServer.Tests.Tests.Controllers.TST;

public class TstGroupControllerTests
{
    private readonly Mock<IGroupService<TstGroup>> _mockGroupService;
    private readonly TstMapper _mapper;
    private readonly TstGroupController _controller;

    public TstGroupControllerTests()
    {
        _mockGroupService = new Mock<IGroupService<TstGroup>>();
        _mapper = new TstMapper();
        _controller = new TstGroupController(_mockGroupService.Object, _mapper);
    }


    [Fact]
    public async Task GetAllGroups_WithExistingGroups_ReturnsOkWithGroups()
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

        // Act
        var result = await _controller.GetAllGroups();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroups = Assert.IsAssignableFrom<IEnumerable<TstGroupResponseDto>>(okResult.Value).ToList();
        Assert.Equal(2, returnedGroups.Count);
        Assert.Equal("group1", returnedGroups.First().Id);
        Assert.Equal("Engineering", returnedGroups.First().DisplayName);
        
        _mockGroupService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllGroups_WithNoGroups_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<TstGroup>());

        // Act
        var result = await _controller.GetAllGroups();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroups = Assert.IsAssignableFrom<IEnumerable<TstGroupResponseDto>>(okResult.Value);
        Assert.Empty(returnedGroups);
        
        _mockGroupService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllGroups_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllGroups();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockGroupService.Verify(s => s.GetAllAsync(), Times.Once);
    }


    [Fact]
    public async Task GetGroupById_WithExistingGroup_ReturnsOkWithGroup()
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

        // Act
        var result = await _controller.GetGroupById("group1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroup = Assert.IsType<TstGroupResponseDto>(okResult.Value);
        Assert.Equal("group1", returnedGroup.Id);
        Assert.Equal("Engineering", returnedGroup.DisplayName);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task GetGroupById_WithNonExistentGroup_ReturnsNotFound()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((TstGroup?)null);

        // Act
        var result = await _controller.GetGroupById("nonexistent");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("nonexistent"), Times.Once);
    }

    [Fact]
    public async Task GetGroupById_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetGroupById("group1");

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
    }


    [Fact]
    public async Task CreateGroup_WithValidGroup_ReturnsCreatedAtAction()
    {
        // Arrange
        var requestDto = new TstGroupRequestDto(
            "group1",
            "Engineering",
            "ext1",
            "ext2"
        );

        _mockGroupService.Setup(s => s.AddAsync(It.IsAny<TstGroup>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateGroup(requestDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TstGroupController.GetGroupById), createdAtActionResult.ActionName);
        
        var returnedGroup = Assert.IsType<TstGroupResponseDto>(createdAtActionResult.Value);
        Assert.Equal("group1", returnedGroup.Id);
        Assert.Equal("Engineering", returnedGroup.DisplayName);
        
        _mockGroupService.Verify(s => s.AddAsync(It.Is<TstGroup>(g => 
            g.Id == "group1" && 
            g.DisplayName == "Engineering"
        )), Times.Once);
    }

    [Fact]
    public async Task CreateGroup_WithInvalidGroup_ReturnsBadRequest()
    {
        // Arrange
        var requestDto = new TstGroupRequestDto(
            "",
            "Engineering",
            "ext1",
            "ext2"
        );

        _mockGroupService.Setup(s => s.AddAsync(It.IsAny<TstGroup>()))
            .ThrowsAsync(new ArgumentException("Id cannot be empty"));

        // Act
        var result = await _controller.CreateGroup(requestDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequestResult.Value);
        
        _mockGroupService.Verify(s => s.AddAsync(It.IsAny<TstGroup>()), Times.Once);
    }

    [Fact]
    public async Task CreateGroup_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var requestDto = new TstGroupRequestDto(
            "group1",
            "Engineering",
            "ext1",
            "ext2"
        );

        _mockGroupService.Setup(s => s.AddAsync(It.IsAny<TstGroup>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateGroup(requestDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockGroupService.Verify(s => s.AddAsync(It.IsAny<TstGroup>()), Times.Once);
    }


    [Fact]
    public async Task UpdateGroup_WithValidGroupAndMatchingId_ReturnsNoContent()
    {
        // Arrange
        var requestDto = new TstGroupRequestDto(
            "group1",
            "Engineering Updated",
            "ext1",
            "ext2"
        );

        var existingGroup = new TstGroup
        {
            Id = "group1",
            DisplayName = "Engineering",
            TstGroupExtension1 = "ext1",
            TstGroupExtension2 = "ext2"
        };

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync(existingGroup);
        _mockGroupService.Setup(s => s.UpdateAsync(It.IsAny<TstGroup>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateGroup("group1", requestDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
        _mockGroupService.Verify(s => s.UpdateAsync(It.Is<TstGroup>(g => 
            g.Id == "group1" && 
            g.DisplayName == "Engineering Updated"
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateGroup_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var requestDto = new TstGroupRequestDto(
            "group2",
            "Engineering",
            "ext1",
            "ext2"
        );

        // Act
        var result = await _controller.UpdateGroup("group1", requestDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        
        _mockGroupService.Verify(s => s.GetByIdAsync(It.IsAny<string>()), Times.Never);
        _mockGroupService.Verify(s => s.UpdateAsync(It.IsAny<TstGroup>()), Times.Never);
    }

    [Fact]
    public async Task UpdateGroup_WithNonExistentGroup_ReturnsNotFound()
    {
        // Arrange
        var requestDto = new TstGroupRequestDto(
            "group1",
            "Engineering",
            "ext1",
            "ext2"
        );

        _mockGroupService.Setup(s => s.GetByIdAsync("group1")).ReturnsAsync((TstGroup?)null);

        // Act
        var result = await _controller.UpdateGroup("group1", requestDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
        _mockGroupService.Verify(s => s.UpdateAsync(It.IsAny<TstGroup>()), Times.Never);
    }

    [Fact]
    public async Task UpdateGroup_WithValidationError_ReturnsBadRequest()
    {
        // Arrange
        var requestDto = new TstGroupRequestDto(
            "group1",
            "",
            "ext1",
            "ext2"
        );

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

        // Act
        var result = await _controller.UpdateGroup("group1", requestDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        
        _mockGroupService.Verify(s => s.UpdateAsync(It.IsAny<TstGroup>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGroup_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var requestDto = new TstGroupRequestDto(
            "group1",
            "Engineering",
            "ext1",
            "ext2"
        );

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

        // Act
        var result = await _controller.UpdateGroup("group1", requestDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockGroupService.Verify(s => s.UpdateAsync(It.IsAny<TstGroup>()), Times.Once);
    }


    [Fact]
    public async Task DeleteGroup_WithExistingGroup_ReturnsNoContent()
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

        // Act
        var result = await _controller.DeleteGroup("group1");

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("group1"), Times.Once);
        _mockGroupService.Verify(s => s.DeleteAsync("group1"), Times.Once);
    }

    [Fact]
    public async Task DeleteGroup_WithNonExistentGroup_ReturnsNotFound()
    {
        // Arrange
        _mockGroupService.Setup(s => s.GetByIdAsync("nonexistent")).ReturnsAsync((TstGroup?)null);

        // Act
        var result = await _controller.DeleteGroup("nonexistent");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
        
        _mockGroupService.Verify(s => s.GetByIdAsync("nonexistent"), Times.Once);
        _mockGroupService.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteGroup_ServiceThrowsException_ReturnsInternalServerError()
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

        // Act
        var result = await _controller.DeleteGroup("group1");

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        
        _mockGroupService.Verify(s => s.DeleteAsync("group1"), Times.Once);
    }
}



