using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Generic;

namespace GrpcServer.Infrastructure.Services.Generic;

public interface IGroupService
{
    Task<AppCode> GetServiceAppCodeAsync();
    Task<IBaseGroup?> GetByIdAsync(int id);
    Task<IEnumerable<IBaseGroup>> GetAllAsync();
    Task AddAsync(IBaseGroup baseGroup);
    Task UpdateAsync(IBaseGroup baseGroup);
    Task DeleteAsync(int id);
}