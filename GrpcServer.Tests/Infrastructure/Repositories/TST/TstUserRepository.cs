using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Repositories.TST;

public class TstUserRepository : IUserRepository
{
    private readonly Dictionary<string, TstUser> _users = new(); // In-memory database for testing

    public Task<IBaseUser?> GetByIdAsync(string id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult<IBaseUser?>(user);
    }

    public Task<IEnumerable<IBaseUser>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<IBaseUser>>(_users.Values.ToList());
    }

    public Task AddAsync(IBaseUser baseUser)
    {
        if (baseUser is not TstUser tstUser)
        {
            throw new ArgumentException("Only TstUser instances are supported by this repository.", nameof(baseUser));
        }
        
        if (_users.ContainsKey(tstUser.Id))
        {
            throw new InvalidOperationException($"User with ID '{tstUser.Id}' already exists.");
        }
        _users[tstUser.Id] = tstUser;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IBaseUser baseUser)
    {
        if (baseUser is not TstUser tstUser)
        {
            throw new ArgumentException("Only TstUser instances are supported by this repository.", nameof(baseUser));
        }
        
        if (!_users.ContainsKey(tstUser.Id))
        {
            throw new InvalidOperationException($"User with ID '{tstUser.Id}' not found.");
        }
        _users[tstUser.Id] = tstUser;
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

