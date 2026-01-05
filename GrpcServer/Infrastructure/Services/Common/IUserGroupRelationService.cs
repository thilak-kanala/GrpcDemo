using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Services.Common;

public interface IUserGroupRelationService
{
    Task<AppCode> GetServiceAppCodeAsync();
    Task<IEnumerable<IBaseGroup>> GetUserGroupsAsync(string userId);
    Task AddUserToGroupsAsync(string userId, List<string> groupIds);
    Task RemoveUserFromGroupAsync(string userId, string groupId);
    Task<IEnumerable<IBaseUser>> GetGroupUsersAsync(string groupId);
    Task AddUsersToGroupAsync(string groupId, List<string> userIds);
}

