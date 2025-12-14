using GrpcServer.Infrastructure.Models;
using GrpcServer.Infrastructure.Repositories;

namespace GrpcServer.Infrastructure.Services;

public class UserService(IUserRepository userRepository)
{
    public Task<IUser?> GetByIdAsync(int id) => userRepository.GetByIdAsync(id);
    public Task<IEnumerable<IUser>> GetAllAsync() => userRepository.GetAllAsync();
    public Task AddAsync(IUser user) => userRepository.AddAsync(user);
    public Task UpdateAsync(IUser user) => userRepository.UpdateAsync(user);
    public Task DeleteAsync(int id) => userRepository.DeleteAsync(id);
}