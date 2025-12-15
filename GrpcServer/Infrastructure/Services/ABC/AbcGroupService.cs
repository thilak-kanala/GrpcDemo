using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Generic;
using GrpcServer.Infrastructure.Repositories.Generic;
using GrpcServer.Infrastructure.Services.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServer.Infrastructure.Services.ABC;

public class AbcGroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;

    public AbcGroupService([FromKeyedServices(AppCode.ABC)] IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public Task<AppCode> GetServiceAppCodeAsync()
    {
        return Task.FromResult(AppCode.ABC);
    }

    public Task<IBaseGroup?> GetByIdAsync(int id)
    {
        return _groupRepository.GetByIdAsync(id);
    }

    public Task<IEnumerable<IBaseGroup>> GetAllAsync()
    {
        return _groupRepository.GetAllAsync();
    }

    public Task AddAsync(IBaseGroup baseGroup)
    {
        return _groupRepository.AddAsync(baseGroup);
    }

    public Task UpdateAsync(IBaseGroup baseGroup)
    {
        return _groupRepository.UpdateAsync(baseGroup);
    }

    public Task DeleteAsync(int id)
    {
        return _groupRepository.DeleteAsync(id);
    }
}

