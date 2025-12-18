namespace GrpcServer.Infrastructure.Repositories.Common;

public interface IUserGroupRelationRepository
{
    Task<IEnumerable<int>> GetGroupIdsByUserIdAsync(int userId);
    Task<IEnumerable<int>> GetUserIdsByGroupIdAsync(int groupId);
    Task AddUserToGroupAsync(int userId, int groupId);
    Task RemoveUserFromGroupAsync(int userId, int groupId);
}

