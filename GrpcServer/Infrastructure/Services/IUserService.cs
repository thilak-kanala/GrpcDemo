using GrpcServer.Models;
using GrpcServer.Models.Request;

namespace GrpcServer.Infrastructure.Services;

public interface IUserService
{
    // TODO: is validation needed here? if yes, add another boolean parameter to indicate whether to validate or not
    Task<User?> CreateUserAsync(User user, ConnectionParametersApi connectionParametersApi);
}