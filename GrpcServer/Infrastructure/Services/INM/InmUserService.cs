using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Generic;
using GrpcServer.Infrastructure.Repositories.Generic;
using GrpcServer.Infrastructure.Services.Generic;

namespace GrpcServer.Infrastructure.Services.INM;

public class InmUserService([FromKeyedServices(AppCode.INM)] IUserRepository userRepository)
    : IUserService
{
    public Task<AppCode> GetServiceAppCodeAsync()
    {
        return Task.FromResult(AppCode.INM);
    }
    
    public Task<IBaseUser?> GetByIdAsync(int id)
    {
        return userRepository.GetByIdAsync(id);
    }

    public Task<IEnumerable<IBaseUser>> GetAllAsync()
    {
        return userRepository.GetAllAsync();
    }

    public Task AddAsync(IBaseUser baseUser)
    {
        return userRepository.AddAsync(baseUser);
    }

    public Task UpdateAsync(IBaseUser baseUser)
    {
        return userRepository.UpdateAsync(baseUser);
    }

    public Task DeleteAsync(int id)
    {
        return userRepository.DeleteAsync(id);
    }
}
