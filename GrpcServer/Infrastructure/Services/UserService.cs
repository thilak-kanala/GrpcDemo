using GrpcServer.Models;
using GrpcServer.Models.Request;

namespace GrpcServer.Infrastructure.Services;

public class UserService(ILogger<UserService> logger) : IUserService
{
    public Task<User?> CreateUserAsync(User user, ConnectionParametersApi connectionParametersApi)
    {
        logger.LogDebug("Started CreateUserAsync(user={@user}, connectionParams={@connectionParametersApi})", user,
            connectionParametersApi);

        try
        {
            // TODO: Implement actual user creation logic

            // Mock implementation: return the original user info as a completed task
            return Task.FromResult<User?>(user);
        }
        finally
        {
            logger.LogDebug("Finished CreateUserAsync(user={@user}, connectionParams={@connectionParametersApi})",
                user, connectionParametersApi);
        }
    }
}