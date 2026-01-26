using System.Collections.Concurrent;
using System.Text.Json;
using Tst2TargetApplication.Infrastructure.Models;

namespace Tst2TargetApplication.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _dataFilePath;
    private readonly ConcurrentDictionary<int, User> _users;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public UserRepository(IWebHostEnvironment environment)
    {
        _dataFilePath = Path.Combine(environment.ContentRootPath, "Resources", "UsersMockData.json");
        _users = new ConcurrentDictionary<int, User>(LoadUsersFromFile().ToDictionary(u => u.Id));
    }

    private List<User> LoadUsersFromFile()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<User>();
        }

        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    private async Task SaveUsersToFileAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(_users.Values.OrderBy(u => u.Id), new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(_dataFilePath, json);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        return Task.FromResult(_users.Values.AsEnumerable());
    }

    public Task<User?> GetByIdAsync(int id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = _users.Values.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetByGroupIdAsync(int groupId)
    {
        var users = _users.Values.Where(u => u.GroupIds.Contains(groupId));
        return Task.FromResult(users.AsEnumerable());
    }

    public async Task<User> CreateAsync(User user)
    {
        var maxId = _users.Keys.Any() ? _users.Keys.Max() : 0;
        user.Id = maxId + 1;
        _users[user.Id] = user;
        await SaveUsersToFileAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(int id, User user)
    {
        if (!_users.TryGetValue(id, out var existingUser))
        {
            return null;
        }

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.IsActive = user.IsActive;
        existingUser.Devices = user.Devices;
        existingUser.PreferredLanguage = user.PreferredLanguage;
        existingUser.GroupIds = user.GroupIds;

        await SaveUsersToFileAsync();
        return existingUser;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!_users.TryRemove(id, out _))
        {
            return false;
        }

        await SaveUsersToFileAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id)
    {
        return Task.FromResult(_users.ContainsKey(id));
    }
}
