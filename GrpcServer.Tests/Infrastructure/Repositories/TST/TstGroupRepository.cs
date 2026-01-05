using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Repositories.TST;

public class TstGroupRepository : IGroupRepository
{
    private readonly Dictionary<string, TstGroup> _groups = new(); // In-memory database for testing

    public Task<IBaseGroup?> GetByIdAsync(string id)
    {
        _groups.TryGetValue(id, out var group);
        return Task.FromResult<IBaseGroup?>(group);
    }

    public Task<IEnumerable<IBaseGroup>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<IBaseGroup>>(_groups.Values.ToList());
    }

    public Task AddAsync(IBaseGroup baseGroup)
    {
        if (baseGroup is not TstGroup tstGroup)
        {
            throw new ArgumentException("Only TstGroup instances are supported by this repository.", nameof(baseGroup));
        }
        
        if (_groups.ContainsKey(tstGroup.Id))
        {
            throw new InvalidOperationException($"Group with ID '{tstGroup.Id}' already exists.");
        }
        _groups[tstGroup.Id] = tstGroup;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IBaseGroup baseGroup)
    {
        if (baseGroup is not TstGroup tstGroup)
        {
            throw new ArgumentException("Only TstGroup instances are supported by this repository.", nameof(baseGroup));
        }
        
        if (!_groups.ContainsKey(tstGroup.Id))
        {
            throw new InvalidOperationException($"Group with ID '{tstGroup.Id}' not found.");
        }
        _groups[tstGroup.Id] = tstGroup;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        if (!_groups.ContainsKey(id))
        {
            throw new InvalidOperationException($"Group with ID '{id}' not found.");
        }
        _groups.Remove(id);
        return Task.CompletedTask;
    }
}

