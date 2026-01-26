using System.Collections.Concurrent;
using System.Text.Json;
using Tst2TargetApplication.Infrastructure.Models;

namespace Tst2TargetApplication.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly string _dataFilePath;
    private readonly ConcurrentDictionary<int, Group> _groups;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public GroupRepository(IWebHostEnvironment environment)
    {
        _dataFilePath = Path.Combine(environment.ContentRootPath, "Resources", "GroupsMockData.json");
        _groups = new ConcurrentDictionary<int, Group>(LoadGroupsFromFile().ToDictionary(g => g.Id));
    }

    private List<Group> LoadGroupsFromFile()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<Group>();
        }

        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<Group>>(json) ?? new List<Group>();
    }

    private async Task SaveGroupsToFileAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(_groups.Values.OrderBy(g => g.Id), new JsonSerializerOptions
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

    public Task<IEnumerable<Group>> GetAllAsync()
    {
        return Task.FromResult(_groups.Values.AsEnumerable());
    }

    public Task<Group?> GetByIdAsync(int id)
    {
        _groups.TryGetValue(id, out var group);
        return Task.FromResult(group);
    }

    public Task<Group?> GetByNameAsync(string name)
    {
        var group = _groups.Values.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(group);
    }

    public Task<IEnumerable<Group>> GetByPriorityAsync(string priority)
    {
        var groups = _groups.Values.Where(g => g.Priority.Equals(priority, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(groups.AsEnumerable());
    }

    public async Task<Group> CreateAsync(Group group)
    {
        var maxId = _groups.Keys.Any() ? _groups.Keys.Max() : 0;
        group.Id = maxId + 1;
        _groups[group.Id] = group;
        await SaveGroupsToFileAsync();
        return group;
    }

    public async Task<Group?> UpdateAsync(int id, Group group)
    {
        if (!_groups.TryGetValue(id, out var existingGroup))
        {
            return null;
        }

        existingGroup.Name = group.Name;
        existingGroup.Description = group.Description;
        existingGroup.Priority = group.Priority;
        existingGroup.NotificationChannels = group.NotificationChannels;

        await SaveGroupsToFileAsync();
        return existingGroup;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!_groups.TryRemove(id, out _))
        {
            return false;
        }

        await SaveGroupsToFileAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id)
    {
        return Task.FromResult(_groups.ContainsKey(id));
    }
}
