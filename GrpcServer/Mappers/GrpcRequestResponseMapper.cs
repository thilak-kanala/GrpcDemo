using GrpcServer.Models;
using GrpcServer.Models.Request;

namespace GrpcServer.Mappers;

/// <summary>
/// Mapper for gRPC request and response messages to domain models and vice versa
/// </summary>
public static class GrpcRequestResponseMapper
{
    /// <summary>
    /// Maps CreateUserRequest gRPC message to User and ConnectionParametersApi models
    /// </summary>
    public static (User user, ConnectionParametersApi connectionParams) FromCreateUserRequest(CreateUserRequest request)
    {
        var user = FromDto(request.User);
        var connectionParams = FromDto(request.ConnectionParameters);
        return (user, connectionParams);
    }
    
    /// <summary>
    /// Maps UserDto gRPC message to User domain model
    /// </summary>
    public static User FromDto(UserDto userDto)
    {
        return new User(
            id: userDto.Id,
            userName: userDto.UserName,
            isActive: userDto.IsActive
        );
    }
    
    /// <summary>
    /// Maps ConnectionParametersAPI gRPC message to ConnectionParametersApi domain model
    /// </summary>
    public static ConnectionParametersApi FromDto(ConnectionParametersAPI connectionParamsDto)
    {
        return new ConnectionParametersApi(
            baseUrl: connectionParamsDto.BaseUrl,
            username: connectionParamsDto.Username,
            password: connectionParamsDto.Password
        );
    }

    /// <summary>
    /// Maps User domain model to UserDto gRPC message
    /// </summary>
    public static UserDto? ToDto(User? user)
    {
        if (user == null) return null;
        
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            IsActive = user.IsActive
        };
    }
    
    /// <summary>
    /// Maps ConnectionParametersApi domain model to ConnectionParametersAPI gRPC message
    /// </summary>
    public static ConnectionParametersAPI ToDto(ConnectionParametersApi connectionParams)
    {
        return new ConnectionParametersAPI
        {
            BaseUrl = connectionParams.BaseUrl,
            Username = connectionParams.Username,
            Password = connectionParams.Password
        };
    }
}
