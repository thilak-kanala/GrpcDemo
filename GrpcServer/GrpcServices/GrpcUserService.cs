using Grpc.Core;
using GrpcServer.Infrastructure.Services;
using GrpcServer.Mappers;

namespace GrpcServer.GrpcServices
{
    public class GrpcUserService(IUserService userService, ILogger<GrpcUserService> logger)
        : ScimUserConnector.ScimUserConnectorBase
    {
        public override async Task<UserDto> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            logger.LogDebug("Started CreateUserAsync(request={@request}", request);
            try
            {
                var (userModel, connectionParams) = GrpcRequestResponseMapper.FromCreateUserRequest(request);

                var createdUser = await userService.CreateUserAsync(userModel, connectionParams);
                var userDto = GrpcRequestResponseMapper.ToDto(createdUser);
                return userDto!;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred while creating user: {Message}", e.Message);
                throw;
            }
            finally
            {
                logger.LogDebug("Finished CreateUserAsync(userDto={userName}", request.User.UserName);
            }
        }
    }
}