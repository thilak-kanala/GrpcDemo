using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Generic;

namespace GrpcServer.Infrastructure.Services.Generic;

public interface IUserService
{
    Task<AppCode> GetServiceAppCodeAsync();
    Task<IBaseUser?> GetByIdAsync(int id);
    Task<IEnumerable<IBaseUser>> GetAllAsync();
    Task AddAsync(IBaseUser baseUser);
    Task UpdateAsync(IBaseUser baseUser);
    Task DeleteAsync(int id);
}