using GrpcServer.Infrastructure.Models;

namespace GrpcServer.Infrastructure.Services;

public class UserGroupRelationService : IUserGroupRelationService
{
    // Mock in-memory storage for relations
    private readonly Dictionary<int, HashSet<int>> _userGroups = new(); // userId -> groupIds
    private readonly Dictionary<int, HashSet<int>> _groupUsers = new(); // groupId -> userIds

    public Task<IEnumerable<IGroup>> GetUserGroupsAsync(int userId)
    {
        // Mock implementation - returns dummy groups
        if (!_userGroups.ContainsKey(userId))
            _userGroups[userId] = new HashSet<int>();

        var groups = _userGroups[userId]
            .Select(gid => new Group { Id = gid, DisplayName = $"Group {gid}" })
            .Cast<IGroup>()
            .ToList();

        return Task.FromResult<IEnumerable<IGroup>>(groups);
    }

    public Task AddUserToGroupsAsync(int userId, List<int> groupIds)
    {
        if (!_userGroups.ContainsKey(userId))
            _userGroups[userId] = new HashSet<int>();

        foreach (var groupId in groupIds)
        {
            _userGroups[userId].Add(groupId);
            
            if (!_groupUsers.ContainsKey(groupId))
                _groupUsers[groupId] = new HashSet<int>();
            
            _groupUsers[groupId].Add(userId);
        }

        return Task.CompletedTask;
    }

    public Task RemoveUserFromGroupAsync(int userId, int groupId)
    {
        if (_userGroups.ContainsKey(userId))
            _userGroups[userId].Remove(groupId);

        if (_groupUsers.ContainsKey(groupId))
            _groupUsers[groupId].Remove(userId);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<IUser>> GetGroupUsersAsync(int groupId)
    {
        // Mock implementation - returns dummy users
        if (!_groupUsers.ContainsKey(groupId))
            _groupUsers[groupId] = new HashSet<int>();

        var users = _groupUsers[groupId]
            .Select(uid => new User { Id = uid, UserName = $"User{uid}", Email = $"user{uid}@example.com" })
            .Cast<IUser>()
            .ToList();

        return Task.FromResult<IEnumerable<IUser>>(users);
    }

    public Task AddUsersToGroupAsync(int groupId, List<int> userIds)
    {
        if (!_groupUsers.ContainsKey(groupId))
            _groupUsers[groupId] = new HashSet<int>();

        foreach (var userId in userIds)
        {
            _groupUsers[groupId].Add(userId);
            
            if (!_userGroups.ContainsKey(userId))
                _userGroups[userId] = new HashSet<int>();
            
            _userGroups[userId].Add(groupId);
        }

        return Task.CompletedTask;
    }

    public Task RemoveUserFromGroupInGroupContextAsync(int groupId, int userId)
    {
        return RemoveUserFromGroupAsync(userId, groupId);
    }
}

