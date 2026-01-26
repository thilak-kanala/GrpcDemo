using Tst2TargetApplication.Infrastructure.Models;

namespace Tst2TargetApplication.Infrastructure.Repositories;

public interface IGroupRepository
{
    Task<IEnumerable<Group>> GetAllAsync();
    Task<Group?> GetByIdAsync(int id);
    Task<Group?> GetByNameAsync(string name);
    Task<IEnumerable<Group>> GetByPriorityAsync(string priority);
    Task<Group> CreateAsync(Group group);
    Task<Group?> UpdateAsync(int id, Group group);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
