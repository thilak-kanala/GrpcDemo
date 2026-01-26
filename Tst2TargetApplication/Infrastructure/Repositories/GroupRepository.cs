using System.Text.Json;
using Tst2TargetApplication.Infrastructure.Models;

namespace Tst2TargetApplication.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly string _dataFilePath;
    private List<Group> _groups;

    public GroupRepository(IWebHostEnvironment environment)
    {
        _dataFilePath = Path.Combine(environment.ContentRootPath, "Infrastructure", "Util", "GroupsMockData.json");
        _groups = LoadGroupsFromFile();
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
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_groups, options);
        await File.WriteAllTextAsync(_dataFilePath, json);
    }

    public async Task<IEnumerable<Group>> GetAllGroupsAsync()
    {
        return _groups.ToList();
    }

    public async Task<Group?> GetGroupByIdAsync(int id)
    {
        return _groups.FirstOrDefault(g => g.Id == id);
    }

    public async Task<Group> CreateGroupAsync(Group group)
    {
        var newId = _groups.Any() ? _groups.Max(g => g.Id) + 1 : 1;
        var newGroup = new Group
        {
            Id = newId,
            Name = group.Name,
            Description = group.Description,
            Priority = group.Priority
        };

        _groups.Add(newGroup);
        await SaveGroupsToFileAsync();

        return newGroup;
    }

    public async Task<Group?> ReplaceGroupAsync(int id, Group group)
    {
        var existingGroupIndex = _groups.FindIndex(g => g.Id == id);
        if (existingGroupIndex == -1) return null;

        var updatedGroup = new Group
        {
            Id = id,
            Name = group.Name,
            Description = group.Description,
            Priority = group.Priority
        };
        _groups[existingGroupIndex] = updatedGroup;

        await SaveGroupsToFileAsync();

        return updatedGroup;
    }

    public async Task<Group?> UpdateGroupAsync(int id, Group group)
    {
        var existingGroup = _groups.FirstOrDefault(g => g.Id == id);
        if (existingGroup == null) return null;

        // PATCH - only update non-null/non-default values
        if (!string.IsNullOrEmpty(group.Name))
            existingGroup.Name = group.Name;
        if (!string.IsNullOrEmpty(group.Description))
            existingGroup.Description = group.Description;
        if (!string.IsNullOrEmpty(group.Priority))
            existingGroup.Priority = group.Priority;

        await SaveGroupsToFileAsync();

        return existingGroup;
    }

    public async Task<bool> DeleteGroupAsync(int id)
    {
        var group = _groups.FirstOrDefault(g => g.Id == id);
        if (group == null) return false;

        _groups.Remove(group);
        await SaveGroupsToFileAsync();
        return true;
    }
}

