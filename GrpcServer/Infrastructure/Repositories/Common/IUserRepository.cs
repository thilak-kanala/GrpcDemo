using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Repositories.Common;

/// <summary>
/// Repository interface for managing user entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user. Cannot be null or empty.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<IBaseUser?> GetByIdAsync(string id);
    
    /// <summary>
    /// Retrieves all users from the repository.
    /// </summary>
    /// <returns>A collection of all users. Returns an empty collection if no users exist.</returns>
    Task<IEnumerable<IBaseUser>> GetAllAsync();
    
    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="baseUser">The user to add. Cannot be null. All required properties must be set.</param>
    Task AddAsync(IBaseUser baseUser);
    
    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="baseUser">The user with updated values. Cannot be null. All required properties must be set.</param>
    Task UpdateAsync(IBaseUser baseUser);
    
    /// <summary>
    /// Deletes a user from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete. Cannot be null or empty.</param>
    Task DeleteAsync(string id);
}