using GrpcServer.Infrastructure.Models;
using GrpcServer.Infrastructure.Repositories;

namespace GrpcServer.Infrastructure.Services;

public class GroupService(IGroupRepository groupRepository)
{
    public Task<IGroup?> GetByIdAsync(int id) => groupRepository.GetByIdAsync(id);
    public Task<IEnumerable<IGroup>> GetAllAsync() => groupRepository.GetAllAsync();
    public Task AddAsync(IGroup group) => groupRepository.AddAsync(group);
    public Task UpdateAsync(IGroup group) => groupRepository.UpdateAsync(group);
    public Task DeleteAsync(int id) => groupRepository.DeleteAsync(id);
}