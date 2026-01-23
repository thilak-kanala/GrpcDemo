using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Protos.Common;
using GrpcServer.Protos.TST;

namespace GrpcServer.Tests.Tests.Mappers.TST;

public class TstProtoMapperTests
{
    private readonly TstProtoMapper _mapper;

    public TstProtoMapperTests()
    {
        _mapper = new TstProtoMapper();
    }

    [Fact]
    public void ToMessage_WithTstUser_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var user = new TstUser
        {
            Id = "user123",
            UserName = "john.doe",
            Email = "john@example.com",
            TstUserExtension1 = "Extension1Value",
            TstUserExtension2 = "Extension2Value"
        };

        // Act
        var message = _mapper.ToMessage(user);

        // Assert
        Assert.NotNull(message);
        Assert.NotNull(message.Base);
        Assert.Equal("user123", message.Base.Id);
        Assert.Equal("john.doe", message.Base.UserName);
        Assert.Equal("john@example.com", message.Base.Email);
        Assert.Equal("Extension1Value", message.TstUserExtension1);
        Assert.Equal("Extension2Value", message.TstUserExtension2);
    }

    [Fact]
    public void FromRequest_WithTstUserRequest_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var request = new TstUserRequest
        {
            Base = new BaseUserMessage
            {
                Id = "user789",
                UserName = "bob.jones",
                Email = "bob@example.com"
            },
            TstUserExtension1 = "CustomValue1",
            TstUserExtension2 = "CustomValue2"
        };

        // Act
        var user = _mapper.FromRequest(request);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("user789", user.Id);
        Assert.Equal("bob.jones", user.UserName);
        Assert.Equal("bob@example.com", user.Email);
        Assert.Equal("CustomValue1", user.TstUserExtension1);
        Assert.Equal("CustomValue2", user.TstUserExtension2);
    }

    [Fact]
    public void ToMessage_WithTstGroup_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var group = new TstGroup
        {
            Id = "group123",
            DisplayName = "Engineering Team",
            TstGroupExtension1 = "GroupExt1",
            TstGroupExtension2 = "GroupExt2"
        };

        // Act
        var message = _mapper.ToMessage(group);

        // Assert
        Assert.NotNull(message);
        Assert.NotNull(message.Base);
        Assert.Equal("group123", message.Base.Id);
        Assert.Equal("Engineering Team", message.Base.DisplayName);
        Assert.Equal("GroupExt1", message.TstGroupExtension1);
        Assert.Equal("GroupExt2", message.TstGroupExtension2);
    }

    [Fact]
    public void FromRequest_WithTstGroupRequest_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var request = new TstGroupRequest
        {
            Base = new BaseGroupMessage
            {
                Id = "group789",
                DisplayName = "Sales Team"
            },
            TstGroupExtension1 = "SalesExt1",
            TstGroupExtension2 = "SalesExt2"
        };

        // Act
        var group = _mapper.FromRequest(request);

        // Assert
        Assert.NotNull(group);
        Assert.Equal("group789", group.Id);
        Assert.Equal("Sales Team", group.DisplayName);
        Assert.Equal("SalesExt1", group.TstGroupExtension1);
        Assert.Equal("SalesExt2", group.TstGroupExtension2);
    }
}