using Tst2TargetApplication.Infrastructure.Models;

namespace Tst2TargetApplication.Infrastructure.Repositories;

public interface IGroupRepository
{
    Task<IEnumerable<Group>> GetAllGroupsAsync();
    Task<Group?> GetGroupByIdAsync(int id);
    Task<Group> CreateGroupAsync(Group group);
    Task<Group?> ReplaceGroupAsync(int id, Group group);
    Task<Group?> UpdateGroupAsync(int id, Group group);
    Task<bool> DeleteGroupAsync(int id);
}

