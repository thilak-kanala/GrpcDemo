namespace GrpcServer.Infrastructure.Repositories.Common;

/// <summary>
/// Repository interface for managing relationships between users and groups.
/// </summary>
public interface IUserGroupRelationRepository
{
    /// <summary>
    /// Retrieves all group IDs that a user belongs to.
    // / </summary>
    /// <param name="userId">The unique identifier of the user. Cannot be null or empty.</param>
    /// <returns>A collection of group IDs. Returns an empty collection if the user belongs to no groups.</returns>
    Task<IEnumerable<string>> GetGroupIdsByUserIdAsync(string userId);
    
    /// <summary>
    /// Retrieves all user IDs that belong to a group.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group. Cannot be null or empty.</param>
    /// <returns>A collection of user IDs. Returns an empty collection if the group has no members.</returns>
    Task<IEnumerable<string>> GetUserIdsByGroupIdAsync(string groupId);
    
    /// <summary>
    /// Adds a user to a group, creating a relationship between them.
    /// </summary>
    /// <param name="userId">The unique identifier of the user. Cannot be null or empty.</param>
    /// <param name="groupId">The unique identifier of the group. Cannot be null or empty.</param>
    Task AddUserToGroupAsync(string userId, string groupId);
    
    /// <summary>
    /// Removes a user from a group, deleting the relationship between them.
    /// </summary>
    /// <param name="userId">The unique identifier of the user. Cannot be null or empty.</param>
    /// <param name="groupId">The unique identifier of the group. Cannot be null or empty.</param>
    Task RemoveUserFromGroupAsync(string userId, string groupId);
}

