using GrpcServer.Infrastructure.Models;

namespace GrpcServer.Infrastructure.Repositories;

public class GroupRepository(HttpClient httpClient) : IGroupRepository
{
    private readonly HttpClient _httpClient = httpClient;

    public Task<IGroup?> GetByIdAsync(int id)
    {
        // TODO: Implement API call
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<IGroup>> GetAllAsync()
    {
        // TODO: Implement API call
        throw new System.NotImplementedException();
    }

    public Task AddAsync(IGroup group)
    {
        // TODO: Implement API call
        throw new System.NotImplementedException();
    }

    public Task UpdateAsync(IGroup group)
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