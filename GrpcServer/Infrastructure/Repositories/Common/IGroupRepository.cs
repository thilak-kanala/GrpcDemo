using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Repositories.Common;

/// <summary>
/// Repository interface for managing group entities.
/// </summary>
/// <typeparam name="TGroup">The type of group entity that implements IBaseGroup.</typeparam>
public interface IGroupRepository<TGroup> where TGroup : IBaseGroup
{
    /// <summary>
    /// Retrieves a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group. Cannot be null or empty.</param>
    /// <returns>The group if found; otherwise, null.</returns>
    Task<TGroup?> GetByIdAsync(string id);
    
    /// <summary>
    /// Retrieves all groups from the repository.
    /// </summary>
    /// <returns>A collection of all groups. Returns an empty collection if no groups exist.</returns>
    Task<IEnumerable<TGroup>> GetAllAsync();

    /// <summary>
    /// Adds a new group to the repository.
    /// </summary>
    /// <param name="group">The group to add. Cannot be null. All required properties must be set.</param>
    Task AddAsync(TGroup group);
    
    /// <summary>
    /// Updates an existing group in the repository.
    /// </summary>
    /// <param name="group">The group with updated values. Cannot be null. All required properties must be set.</param>
    Task UpdateAsync(TGroup group);
    
    /// <summary>
    /// Deletes a group from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the group to delete. Cannot be null or empty.</param>
    Task DeleteAsync(string id);
}