using GrpcServer.Infrastructure.Models;

namespace GrpcServer.Infrastructure.Services;

public interface IUserGroupRelationService
{
    Task<IEnumerable<IGroup>> GetUserGroupsAsync(int userId);
    Task AddUserToGroupsAsync(int userId, List<int> groupIds);
    Task RemoveUserFromGroupAsync(int userId, int groupId);
    Task<IEnumerable<IUser>> GetGroupUsersAsync(int groupId);
    Task AddUsersToGroupAsync(int groupId, List<int> userIds);
    Task RemoveUserFromGroupInGroupContextAsync(int groupId, int userId);
}

