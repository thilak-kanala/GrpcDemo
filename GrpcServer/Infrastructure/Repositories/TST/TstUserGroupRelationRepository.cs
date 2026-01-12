using GrpcServer.Infrastructure.Repositories.Common;

namespace GrpcServer.Infrastructure.Repositories.TST;

public class TstUserGroupRelationRepository : IUserGroupRelationRepository
{
    private readonly HashSet<(string UserId, string GroupId)> _relations = new(); // In-memory database for testing

    public Task<IEnumerable<string>> GetGroupIdsByUserIdAsync(string userId)
    {
        var groupIds = _relations
            .Where(r => r.UserId == userId)
            .Select(r => r.GroupId)
            .ToList();
        return Task.FromResult<IEnumerable<string>>(groupIds);
    }

    public Task<IEnumerable<string>> GetUserIdsByGroupIdAsync(string groupId)
    {
        var userIds = _relations
            .Where(r => r.GroupId == groupId)
            .Select(r => r.UserId)
            .ToList();
        return Task.FromResult<IEnumerable<string>>(userIds);
    }

    public Task AddUserToGroupAsync(string userId, string groupId)
    {
        var relation = (userId, groupId);
        if (_relations.Contains(relation))
        {
            throw new InvalidOperationException($"Relation between User '{userId}' and Group '{groupId}' already exists.");
        }
        _relations.Add(relation);
        return Task.CompletedTask;
    }

    public Task RemoveUserFromGroupAsync(string userId, string groupId)
    {
        var relation = (userId, groupId);
        if (!_relations.Contains(relation))
        {
            throw new InvalidOperationException($"Relation between User '{userId}' and Group '{groupId}' not found.");
        }
        _relations.Remove(relation);
        return Task.CompletedTask;
    }
}

