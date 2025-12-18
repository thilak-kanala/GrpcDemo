using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Services.Common;

namespace GrpcServer.Infrastructure.Services.INM;

public class InmGroupService([FromKeyedServices(AppCode.INM)] IGroupRepository groupRepository)
    : IGroupService
{
    public Task<AppCode> GetServiceAppCodeAsync()
    {
        return Task.FromResult(AppCode.INM);
    }

    public Task<IBaseGroup?> GetByIdAsync(int id)
    {
        return groupRepository.GetByIdAsync(id);
    }

    public Task<IEnumerable<IBaseGroup>> GetAllAsync()
    {
        return groupRepository.GetAllAsync();
    }

    public Task AddAsync(IBaseGroup baseGroup)
    {
        return groupRepository.AddAsync(baseGroup);
    }

    public Task UpdateAsync(IBaseGroup baseGroup)
    {
        return groupRepository.UpdateAsync(baseGroup);
    }

    public Task DeleteAsync(int id)
    {
        return groupRepository.DeleteAsync(id);
    }
}

