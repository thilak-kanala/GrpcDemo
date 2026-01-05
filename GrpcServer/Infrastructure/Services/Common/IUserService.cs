using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Services.Common;

public interface IUserService
{
    Task<AppCode> GetServiceAppCodeAsync();
    Task<IBaseUser?> GetByIdAsync(string id);
    Task<IEnumerable<IBaseUser>> GetAllAsync();
    Task AddAsync(IBaseUser baseUser);
    Task UpdateAsync(IBaseUser baseUser);
    Task DeleteAsync(string id);
}