using GrpcServer.Infrastructure.Models;

namespace GrpcServer.Infrastructure.Repositories;

public interface IGroupRepository
{
    Task<IGroup?> GetByIdAsync(int id);
    Task<IEnumerable<IGroup>> GetAllAsync();
    Task AddAsync(IGroup group);
    Task UpdateAsync(IGroup group);
    Task DeleteAsync(int id);
}