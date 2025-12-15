using GrpcServer.Infrastructure.Models.Generic;
using GrpcServer.Infrastructure.Repositories.Generic;
using GrpcServer.Infrastructure.Models.ABC;

namespace GrpcServer.Infrastructure.Repositories.ABC;

public class AbcUserRepository : IUserRepository
{
    private readonly List<AbcUser> _users;
    private int _nextId;

    public AbcUserRepository()
    {
        _users = new List<AbcUser>
        {
            new AbcUser { Id = 1, UserName = "alice.abc", Email = "alice@abc.com", SourceSystem = "ABC_System" },
            new AbcUser { Id = 2, UserName = "bob.abc", Email = "bob@abc.com", SourceSystem = "ABC_System" },
            new AbcUser { Id = 3, UserName = "charlie.abc", Email = "charlie@abc.com", SourceSystem = "ABC_External" },
            new AbcUser { Id = 4, UserName = "diana.abc", Email = "diana@abc.com", SourceSystem = "ABC_System" },
            new AbcUser { Id = 5, UserName = "edward.abc", Email = "edward@abc.com", SourceSystem = "ABC_External" }
        };
        _nextId = 6;
    }

    public Task<IBaseUser?> GetByIdAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult<IBaseUser?>(user);
    }

    public Task<IEnumerable<IBaseUser>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<IBaseUser>>(_users);
    }

    public Task AddAsync(IBaseUser baseUser)
    {
        if (baseUser is not AbcUser user)
        {
            throw new ArgumentException("User must be of type AbcUser", nameof(baseUser));
        }

        if (user.Id == 0)
        {
            user.Id = _nextId++;
        }

        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IBaseUser baseUser)
    {
        if (baseUser is not AbcUser user)
        {
            throw new ArgumentException("User must be of type AbcUser", nameof(baseUser));
        }

        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID {user.Id} not found");
        }

        existingUser.UserName = user.UserName;
        existingUser.Email = user.Email;
        existingUser.SourceSystem = user.SourceSystem;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _users.Remove(user);
        }

        return Task.CompletedTask;
    }
}

