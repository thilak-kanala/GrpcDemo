using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Generic;
using GrpcServer.Infrastructure.Repositories.Generic;
using GrpcServer.Infrastructure.Services.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServer.Infrastructure.Services.ABC;

public class AbcUserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public AbcUserService([FromKeyedServices(AppCode.ABC)] IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<AppCode> GetServiceAppCodeAsync()
    {
        return Task.FromResult(AppCode.ABC);
    }
    
    public Task<IBaseUser?> GetByIdAsync(int id)
    {
        return _userRepository.GetByIdAsync(id);
    }

    public Task<IEnumerable<IBaseUser>> GetAllAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public Task AddAsync(IBaseUser baseUser)
    {
        return _userRepository.AddAsync(baseUser);
    }

    public Task UpdateAsync(IBaseUser baseUser)
    {
        return _userRepository.UpdateAsync(baseUser);
    }

    public Task DeleteAsync(int id)
    {
        return _userRepository.DeleteAsync(id);
    }
}

