using GrpcServer.Infrastructure.Repositories.Common;

namespace GrpcServer.Infrastructure.Repositories.ABC;

public class AbcUserGroupRelationRepository : IUserGroupRelationRepository
{
    // Store relationships as a dictionary: UserId -> List of GroupIds
    private readonly Dictionary<int, HashSet<int>> _userGroupRelations;

    public AbcUserGroupRelationRepository()
    {
        _userGroupRelations = new Dictionary<int, HashSet<int>>
        {
            // Sample data: assigning ABC users to groups
            { 1, new HashSet<int> { 1, 2 } },      // alice.abc -> ABC Engineering, ABC Marketing
            { 2, new HashSet<int> { 1 } },         // bob.abc -> ABC Engineering
            { 3, new HashSet<int> { 3, 4 } },      // charlie.abc -> ABC Sales, ABC HR
            { 4, new HashSet<int> { 2, 4 } },      // diana.abc -> ABC Marketing, ABC HR
            { 5, new HashSet<int> { 5 } }          // edward.abc -> ABC Finance
        };
    }

    public Task<IEnumerable<int>> GetGroupIdsByUserIdAsync(int userId)
    {
        if (_userGroupRelations.TryGetValue(userId, out var groupIds))
        {
            return Task.FromResult<IEnumerable<int>>(groupIds.ToList());
        }
        return Task.FromResult<IEnumerable<int>>(new List<int>());
    }

    public Task<IEnumerable<int>> GetUserIdsByGroupIdAsync(int groupId)
    {
        var userIds = _userGroupRelations
            .Where(kvp => kvp.Value.Contains(groupId))
            .Select(kvp => kvp.Key)
            .ToList();
        return Task.FromResult<IEnumerable<int>>(userIds);
    }

    public Task AddUserToGroupAsync(int userId, int groupId)
    {
        if (!_userGroupRelations.ContainsKey(userId))
        {
            _userGroupRelations[userId] = new HashSet<int>();
        }

        _userGroupRelations[userId].Add(groupId);
        return Task.CompletedTask;
    }

    public Task RemoveUserFromGroupAsync(int userId, int groupId)
    {
        if (_userGroupRelations.TryGetValue(userId, out var groupIds))
        {
            groupIds.Remove(groupId);
            
            // Clean up empty entries
            if (groupIds.Count == 0)
            {
                _userGroupRelations.Remove(userId);
            }
        }

        return Task.CompletedTask;
    }
}

