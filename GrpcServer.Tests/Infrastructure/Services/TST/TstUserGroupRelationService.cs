using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Services.Common;

namespace GrpcServer.Tests.Infrastructure.Services.TST;

/// <summary>
/// TST-specific implementation of IUserGroupRelationService with basic business logic for demonstration.
/// </summary>
public class TstUserGroupRelationService : IUserGroupRelationService
{
    private readonly IUserGroupRelationRepository _relationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public TstUserGroupRelationService(
        IUserGroupRelationRepository relationRepository,
        IUserRepository userRepository,
        IGroupRepository groupRepository)
    {
        _relationRepository = relationRepository;
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }

    public Task<AppCode> GetServiceAppCodeAsync()
    {
        // Return TST app code
        return Task.FromResult(AppCode.Tst);
    }

    public async Task<IEnumerable<IBaseGroup>> GetUserGroupsAsync(string userId)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{userId}' not found.");
        }
        
        // Get group IDs and retrieve full group objects
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

    public async Task AddUserToGroupsAsync(string userId, List<string> groupIds)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{userId}' not found.");
        }
        
        // Add user to each group with validation
        foreach (var groupId in groupIds)
        {
            // Validate group exists
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                throw new InvalidOperationException($"Group with ID '{groupId}' not found.");
            }
            
            await _relationRepository.AddUserToGroupAsync(userId, groupId);
        }
    }

    public async Task RemoveUserFromGroupAsync(string userId, string groupId)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{userId}' not found.");
        }
        
        // Validate group exists
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID '{groupId}' not found.");
        }
        
        await _relationRepository.RemoveUserFromGroupAsync(userId, groupId);
    }

    public async Task<IEnumerable<IBaseUser>> GetGroupUsersAsync(string groupId)
    {
        // Validate group exists
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID '{groupId}' not found.");
        }
        
        // Get user IDs and retrieve full user objects
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

    public async Task AddUsersToGroupAsync(string groupId, List<string> userIds)
    {
        // Validate group exists
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID '{groupId}' not found.");
        }
        
        // Add each user to the group with validation
        foreach (var userId in userIds)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID '{userId}' not found.");
            }
            
            await _relationRepository.AddUserToGroupAsync(userId, groupId);
        }
    }
}

