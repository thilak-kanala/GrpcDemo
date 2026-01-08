using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Services.Common;

/// <summary>
/// Defines the contract for user management operations across different application contexts.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.
    /// </returns>
    Task<IBaseUser?> GetByIdAsync(string id);
    
    /// <summary>
    /// Retrieves all users from the data source.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of all users.
    /// </returns>
    Task<IEnumerable<IBaseUser>> GetAllAsync();
    
    /// <summary>
    /// Adds a new user to the data source.
    /// </summary>
    /// <param name="baseUser">The user entity to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(IBaseUser baseUser);
    
    /// <summary>
    /// Updates an existing user in the data source.
    /// </summary>
    /// <param name="baseUser">The user entity with updated information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(IBaseUser baseUser);
    
    /// <summary>
    /// Deletes a user from the data source by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(string id);
}