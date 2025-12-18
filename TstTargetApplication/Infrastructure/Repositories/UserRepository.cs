using System.Text.Json;
using TstTargetApplication.Infrastructure.Models;

namespace TstTargetApplication.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _dataFilePath;
    private List<UserWithGroupIds> _users;

    public UserRepository(IWebHostEnvironment environment)
    {
        _dataFilePath = Path.Combine(environment.ContentRootPath, "Infrastructure", "Util", "UsersMockData.json");
        _users = LoadUsersFromFile();
    }

    private List<UserWithGroupIds> LoadUsersFromFile()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<UserWithGroupIds>();
        }

        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<UserWithGroupIds>>(json) ?? new List<UserWithGroupIds>();
    }

    private async Task SaveUsersToFileAsync()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_users, options);
        await File.WriteAllTextAsync(_dataFilePath, json);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return _users.Select(u => new User
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PreferredLanguage = u.PreferredLanguage
        }).ToList();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null) return null;

        return new User
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PreferredLanguage = user.PreferredLanguage
        };
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var newId = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
        var newUser = new UserWithGroupIds
        {
            Id = newId,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PreferredLanguage = user.PreferredLanguage,
            GroupIds = new List<int>()
        };

        _users.Add(newUser);
        await SaveUsersToFileAsync();

        return new User
        {
            Id = newUser.Id,
            Username = newUser.Username,
            Email = newUser.Email,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            PreferredLanguage = newUser.PreferredLanguage
        };
    }

    public async Task<User?> ReplaceUserAsync(int id, User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null) return null;

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.PreferredLanguage = user.PreferredLanguage;

        await SaveUsersToFileAsync();

        return new User
        {
            Id = existingUser.Id,
            Username = existingUser.Username,
            Email = existingUser.Email,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
            PreferredLanguage = existingUser.PreferredLanguage
        };
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null) return null;

        // PATCH - only update non-null/non-default values
        if (!string.IsNullOrEmpty(user.Username))
            existingUser.Username = user.Username;
        if (!string.IsNullOrEmpty(user.Email))
            existingUser.Email = user.Email;
        if (!string.IsNullOrEmpty(user.FirstName))
            existingUser.FirstName = user.FirstName;
        if (!string.IsNullOrEmpty(user.LastName))
            existingUser.LastName = user.LastName;
        if (!string.IsNullOrEmpty(user.PreferredLanguage))
            existingUser.PreferredLanguage = user.PreferredLanguage;

        await SaveUsersToFileAsync();

        return new User
        {
            Id = existingUser.Id,
            Username = existingUser.Username,
            Email = existingUser.Email,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
            PreferredLanguage = existingUser.PreferredLanguage
        };
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null) return false;

        _users.Remove(user);
        await SaveUsersToFileAsync();
        return true;
    }

    public async Task<IEnumerable<int>> GetUserGroupIdsAsync(int userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        return user?.GroupIds ?? new List<int>();
    }
}

