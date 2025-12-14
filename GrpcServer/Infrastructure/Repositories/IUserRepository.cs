using GrpcServer.Infrastructure.Models;

namespace GrpcServer.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<IUser?> GetByIdAsync(int id);
    Task<IEnumerable<IUser>> GetAllAsync();
    Task AddAsync(IUser user);
    Task UpdateAsync(IUser user);
    Task DeleteAsync(int id);
}