using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Services.Common;

/// <summary>
/// Defines the contract for group management operations across different application contexts.
/// This service provides CRUD operations for group entities implementing the <see cref="IBaseGroup"/> interface.
/// </summary>
public interface IGroupService
{
    /// <summary>
    /// Retrieves a group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the group if found; otherwise, null.
    /// </returns>
    Task<IBaseGroup?> GetByIdAsync(string id);
    
    /// <summary>
    /// Retrieves all groups from the data source.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of all groups.
    /// </returns>
    Task<IEnumerable<IBaseGroup>> GetAllAsync();
    
    /// <summary>
    /// Adds a new group to the data source.
    /// </summary>
    /// <param name="baseGroup">The group entity to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(IBaseGroup baseGroup);
    
    /// <summary>
    /// Updates an existing group in the data source.
    /// </summary>
    /// <param name="baseGroup">The group entity with updated information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(IBaseGroup baseGroup);
    
    /// <summary>
    /// Deletes a group from the data source by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(string id);
}