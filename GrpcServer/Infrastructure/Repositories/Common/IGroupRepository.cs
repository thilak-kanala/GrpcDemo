using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Repositories.Common;

/// <summary>
/// Repository interface for managing group entities.
/// </summary>
public interface IGroupRepository
{
    /// <summary>
    /// Retrieves a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group. Cannot be null or empty.</param>
    /// <returns>The group if found; otherwise, null.</returns>
    Task<IBaseGroup?> GetByIdAsync(string id);
    
    /// <summary>
    /// Retrieves all groups from the repository.
    /// </summary>
    /// <returns>A collection of all groups. Returns an empty collection if no groups exist.</returns>
    Task<IEnumerable<IBaseGroup>> GetAllAsync();

    /// <summary>
    /// Adds a new group to the repository.
    /// </summary>
    /// <param name="baseGroup">The group to add. Cannot be null. All required properties must be set.</param>
    Task AddAsync(IBaseGroup baseGroup);
    
    /// <summary>
    /// Updates an existing group in the repository.
    /// </summary>
    /// <param name="baseGroup">The group with updated values. Cannot be null. All required properties must be set.</param>
    Task UpdateAsync(IBaseGroup baseGroup);
    
    /// <summary>
    /// Deletes a group from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the group to delete. Cannot be null or empty.</param>
    Task DeleteAsync(string id);
}