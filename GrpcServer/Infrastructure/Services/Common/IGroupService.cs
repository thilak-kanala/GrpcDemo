using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Services.Common;

public interface IGroupService
{
    Task<AppCode> GetServiceAppCodeAsync();
    Task<IBaseGroup?> GetByIdAsync(int id);
    Task<IEnumerable<IBaseGroup>> GetAllAsync();
    Task AddAsync(IBaseGroup baseGroup);
    Task UpdateAsync(IBaseGroup baseGroup);
    Task DeleteAsync(int id);
}