using GrpcServer.Infrastructure.Models;

namespace GrpcServer.Infrastructure.Repositories;

public class UserRepository(HttpClient httpClient) : IUserRepository
{
    private readonly HttpClient _httpClient = httpClient;

    public Task<IUser?> GetByIdAsync(int id)
    {
        // TODO: Implement API call
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<IUser>> GetAllAsync()
    {
        // TODO: Implement API call
        throw new System.NotImplementedException();
    }

    public Task AddAsync(IUser user)
    {
        // TODO: Implement API call
        throw new System.NotImplementedException();
    }

    public Task UpdateAsync(IUser user)
    {
        // TODO: Implement API call
        throw new System.NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        // TODO: Implement API call
        throw new System.NotImplementedException();
    }
}