using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Repositories.Common;

public interface IGroupRepository
{
    Task<IBaseGroup?> GetByIdAsync(int id);
    Task<IEnumerable<IBaseGroup>> GetAllAsync();
    Task AddAsync(IBaseGroup baseGroup);
    Task UpdateAsync(IBaseGroup baseGroup);
    Task DeleteAsync(int id);
}