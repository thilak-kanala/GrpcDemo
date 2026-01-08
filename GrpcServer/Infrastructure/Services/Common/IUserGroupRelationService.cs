using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Services.Common;

/// <summary>
/// Defines the contract for managing relationships between users and groups.
/// This service handles user-group membership operations including adding/removing users to/from groups,
/// and querying group memberships across different application contexts.
/// </summary>
public interface IUserGroupRelationService
{
    /// <summary>
    /// Retrieves the application code that this service is configured to work with.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the <see cref="AppCode"/> enum value
    /// indicating which application context this service instance operates in.
    /// </returns>
    Task<AppCode> GetServiceAppCodeAsync();
    
    /// <summary>
    /// Retrieves all groups that a specific user is a member of.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose group memberships to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of groups
    /// that the specified user belongs to.
    /// </returns>
    Task<IEnumerable<IBaseGroup>> GetUserGroupsAsync(string userId);
    
    /// <summary>
    /// Adds a user to multiple groups in a single operation.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to add to groups.</param>
    /// <param name="groupIds">A list of group identifiers to add the user to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddUserToGroupsAsync(string userId, List<string> groupIds);
    
    /// <summary>
    /// Removes a user from a specific group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to remove from the group.</param>
    /// <param name="groupId">The unique identifier of the group to remove the user from.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveUserFromGroupAsync(string userId, string groupId);
    
    /// <summary>
    /// Retrieves all users that are members of a specific group.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group whose members to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of users
    /// that belong to the specified group.
    /// </returns>
    Task<IEnumerable<IBaseUser>> GetGroupUsersAsync(string groupId);
    
    /// <summary>
    /// Adds multiple users to a group in a single operation.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group to add users to.</param>
    /// <param name="userIds">A list of user identifiers to add to the group.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddUsersToGroupAsync(string groupId, List<string> userIds);
}

