using GrpcServer.Infrastructure.Repositories.Common;

namespace GrpcServer.Infrastructure.Repositories.INM;

public class InmUserGroupRelationRepository : IUserGroupRelationRepository
{
    // Store relationships as a dictionary: UserId -> List of GroupIds
    private readonly Dictionary<int, HashSet<int>> _userGroupRelations;

    public InmUserGroupRelationRepository()
    {
        _userGroupRelations = new Dictionary<int, HashSet<int>>
        {
            // Sample data: assigning users to groups
            { 1, new HashSet<int> { 1, 7 } },      // alice.smith -> Engineering, Product Management
            { 2, new HashSet<int> { 1, 11 } },     // bob.jones -> Engineering, DevOps
            { 3, new HashSet<int> { 10, 11 } },    // charlie.brown -> QA, DevOps
            { 4, new HashSet<int> { 2, 8 } },      // diana.prince -> Marketing, Customer Support
            { 5, new HashSet<int> { 9, 12 } },     // edward.norton -> R&D, Security
            { 6, new HashSet<int> { 4, 5 } },      // fiona.gallagher -> HR, Finance
            { 7, new HashSet<int> { 3 } },         // george.martin -> Sales
            { 8, new HashSet<int> { 8 } },         // hannah.montana -> Customer Support
            { 9, new HashSet<int> { 12 } },        // ivan.drago -> Security
            { 10, new HashSet<int> { 7, 1 } }      // julia.roberts -> Product Management, Engineering
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

