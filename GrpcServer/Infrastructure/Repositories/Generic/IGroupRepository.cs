using GrpcServer.Infrastructure.Models.Generic;

namespace GrpcServer.Infrastructure.Repositories.Generic;

public interface IGroupRepository
{
    Task<IBaseGroup?> GetByIdAsync(int id);
    Task<IEnumerable<IBaseGroup>> GetAllAsync();
    Task AddAsync(IBaseGroup baseGroup);
    Task UpdateAsync(IBaseGroup baseGroup);
    Task DeleteAsync(int id);
}