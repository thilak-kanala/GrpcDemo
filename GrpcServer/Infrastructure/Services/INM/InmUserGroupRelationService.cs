using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Services.Common;

namespace GrpcServer.Infrastructure.Services.INM;

public class InmUserGroupRelationService(
    [FromKeyedServices(AppCode.INM)] IUserRepository userRepository,
    [FromKeyedServices(AppCode.INM)] IGroupRepository groupRepository,
    [FromKeyedServices(AppCode.INM)] IUserGroupRelationRepository relationRepository)
    : IUserGroupRelationService
{
    public Task<AppCode> GetServiceAppCodeAsync()
    {
        return Task.FromResult(AppCode.INM);
    }
    
    public async Task<IEnumerable<IBaseGroup>> GetUserGroupsAsync(int userId)
    {
        // Verify user exists
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        var groupIds = await relationRepository.GetGroupIdsByUserIdAsync(userId);
        var groups = new List<IBaseGroup>();

        foreach (var groupId in groupIds)
        {
            var group = await groupRepository.GetByIdAsync(groupId);
            if (group != null)
            {
                groups.Add(group);
            }
        }

        return groups;
    }

    public async Task AddUserToGroupsAsync(int userId, List<int> groupIds)
    {
        // Verify user exists
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        // Add user to each group
        foreach (var groupId in groupIds)
        {
            // Verify group exists
            var group = await groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                throw new InvalidOperationException($"Group with ID {groupId} not found");
            }

            await relationRepository.AddUserToGroupAsync(userId, groupId);
        }
    }

    public async Task RemoveUserFromGroupAsync(int userId, int groupId)
    {
        // Verify user exists
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        // Verify group exists
        var group = await groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID {groupId} not found");
        }

        await relationRepository.RemoveUserFromGroupAsync(userId, groupId);
    }

    public async Task<IEnumerable<IBaseUser>> GetGroupUsersAsync(int groupId)
    {
        // Verify group exists
        var group = await groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID {groupId} not found");
        }

        var userIds = await relationRepository.GetUserIdsByGroupIdAsync(groupId);
        var users = new List<IBaseUser>();

        foreach (var userId in userIds)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                users.Add(user);
            }
        }

        return users;
    }

    public async Task AddUsersToGroupAsync(int groupId, List<int> userIds)
    {
        // Verify group exists
        var group = await groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID {groupId} not found");
        }

        // Add each user to the group
        foreach (var userId in userIds)
        {
            // Verify user exists
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            await relationRepository.AddUserToGroupAsync(userId, groupId);
        }
    }
}

