using TstTargetApplication.Infrastructure.Models;

namespace TstTargetApplication.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task<User?> ReplaceUserAsync(int id, User user);
    Task<User?> UpdateUserAsync(int id, User user);
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<int>> GetUserGroupIdsAsync(int userId);
}

