using GrpcServer.Infrastructure.Models.Generic;

namespace GrpcServer.Infrastructure.Repositories.Generic;

public interface IUserRepository
{
    Task<IBaseUser?> GetByIdAsync(int id);
    Task<IEnumerable<IBaseUser>> GetAllAsync();
    Task AddAsync(IBaseUser baseUser);
    Task UpdateAsync(IBaseUser baseUser);
    Task DeleteAsync(int id);
}