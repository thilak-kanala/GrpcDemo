using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Repositories.Common;

public interface IUserRepository
{
    Task<IBaseUser?> GetByIdAsync(int id);
    Task<IEnumerable<IBaseUser>> GetAllAsync();
    Task AddAsync(IBaseUser baseUser);
    Task UpdateAsync(IBaseUser baseUser);
    Task DeleteAsync(int id);
}