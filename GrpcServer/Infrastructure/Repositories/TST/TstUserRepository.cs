using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Repositories.Common;

namespace GrpcServer.Infrastructure.Repositories.TST;

public class TstUserRepository : IUserRepository<TstUser>
{
    private readonly Dictionary<string, TstUser> _users = new(); // In-memory database for testing

    public Task<TstUser?> GetByIdAsync(string id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<TstUser>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<TstUser>>(_users.Values.ToList());
    }

    public Task AddAsync(TstUser user)
    {
        if (_users.ContainsKey(user.Id))
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' already exists.");
        }
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TstUser user)
    {
        if (!_users.ContainsKey(user.Id))
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' not found.");
        }
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        if (!_users.ContainsKey(id))
        {
            throw new InvalidOperationException($"User with ID '{id}' not found.");
        }
        _users.Remove(id);
        return Task.CompletedTask;
    }
}

