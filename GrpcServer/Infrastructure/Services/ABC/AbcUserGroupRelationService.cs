using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Services.Common;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServer.Infrastructure.Services.ABC;

public class AbcUserGroupRelationService : IUserGroupRelationService
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRelationRepository _relationRepository;

    public AbcUserGroupRelationService(
        [FromKeyedServices(AppCode.ABC)] IUserRepository userRepository,
        [FromKeyedServices(AppCode.ABC)] IGroupRepository groupRepository,
        [FromKeyedServices(AppCode.ABC)] IUserGroupRelationRepository relationRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _relationRepository = relationRepository;
    }

    public Task<AppCode> GetServiceAppCodeAsync()
    {
        return Task.FromResult(AppCode.ABC);
    }
    
    public async Task<IEnumerable<IBaseGroup>> GetUserGroupsAsync(int userId)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        var groupIds = await _relationRepository.GetGroupIdsByUserIdAsync(userId);
        var groups = new List<IBaseGroup>();

        foreach (var groupId in groupIds)
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
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
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        // Add user to each group
        foreach (var groupId in groupIds)
        {
            // Verify group exists
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                throw new InvalidOperationException($"Group with ID {groupId} not found");
            }

            await _relationRepository.AddUserToGroupAsync(userId, groupId);
        }
    }

    public async Task RemoveUserFromGroupAsync(int userId, int groupId)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        // Verify group exists
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID {groupId} not found");
        }

        await _relationRepository.RemoveUserFromGroupAsync(userId, groupId);
    }

    public async Task<IEnumerable<IBaseUser>> GetGroupUsersAsync(int groupId)
    {
        // Verify group exists
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID {groupId} not found");
        }

        var userIds = await _relationRepository.GetUserIdsByGroupIdAsync(groupId);
        var users = new List<IBaseUser>();

        foreach (var userId in userIds)
        {
            var user = await _userRepository.GetByIdAsync(userId);
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
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID {groupId} not found");
        }

        // Add each user to the group
        foreach (var userId in userIds)
        {
            // Verify user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            await _relationRepository.AddUserToGroupAsync(userId, groupId);
        }
    }
}

