using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Repositories.Common;

namespace GrpcServer.Infrastructure.Repositories.INM;

public class InmUserRepository : IUserRepository
{
    private readonly List<InmUser> _users;
    private int _nextId;

    public InmUserRepository()
    {
        _users = new List<InmUser>
        {
            new InmUser { Id = 1, UserName = "alice.smith", Email = "alice.smith@example.com", InmHost = "dev-server-01" },
            new InmUser { Id = 2, UserName = "bob.jones", Email = "bob.jones@example.com", InmHost = "dev-server-01" },
            new InmUser { Id = 3, UserName = "charlie.brown", Email = "charlie.brown@example.com", InmHost = "dev-server-02" },
            new InmUser { Id = 4, UserName = "diana.prince", Email = "diana.prince@example.com", InmHost = "dev-server-01" },
            new InmUser { Id = 5, UserName = "edward.norton", Email = "edward.norton@example.com", InmHost = "dev-server-02" },
            new InmUser { Id = 6, UserName = "fiona.gallagher", Email = "fiona.gallagher@example.com", InmHost = "dev-server-03" },
            new InmUser { Id = 7, UserName = "george.martin", Email = "george.martin@example.com", InmHost = "dev-server-01" },
            new InmUser { Id = 8, UserName = "hannah.montana", Email = "hannah.montana@example.com", InmHost = "dev-server-02" },
            new InmUser { Id = 9, UserName = "ivan.drago", Email = "ivan.drago@example.com", InmHost = "dev-server-03" },
            new InmUser { Id = 10, UserName = "julia.roberts", Email = "julia.roberts@example.com", InmHost = "dev-server-01" }
        };
        _nextId = 11;
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
        if (baseUser is not InmUser user)
        {
            throw new ArgumentException("User must be of type InmUser", nameof(baseUser));
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
        if (baseUser is not InmUser user)
        {
            throw new ArgumentException("User must be of type InmUser", nameof(baseUser));
        }

        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID {user.Id} not found");
        }

        existingUser.UserName = user.UserName;
        existingUser.Email = user.Email;
        existingUser.InmHost = user.InmHost;

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

