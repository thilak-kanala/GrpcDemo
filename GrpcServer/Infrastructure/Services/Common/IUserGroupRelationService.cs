using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Services.Common;

public interface IUserGroupRelationService
{
    Task<AppCode> GetServiceAppCodeAsync();
    Task<IEnumerable<IBaseGroup>> GetUserGroupsAsync(int userId);
    Task AddUserToGroupsAsync(int userId, List<int> groupIds);
    Task RemoveUserFromGroupAsync(int userId, int groupId);
    Task<IEnumerable<IBaseUser>> GetGroupUsersAsync(int groupId);
    Task AddUsersToGroupAsync(int groupId, List<int> userIds);
}

