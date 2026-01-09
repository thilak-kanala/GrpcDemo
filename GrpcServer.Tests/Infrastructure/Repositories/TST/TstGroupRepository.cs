using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Repositories.TST;

public class TstGroupRepository : IGroupRepository<TstGroup>
{
    private readonly Dictionary<string, TstGroup> _groups = new(); // In-memory database for testing

    public Task<TstGroup?> GetByIdAsync(string id)
    {
        _groups.TryGetValue(id, out var group);
        return Task.FromResult(group);
    }

    public Task<IEnumerable<TstGroup>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<TstGroup>>(_groups.Values.ToList());
    }

    public Task AddAsync(TstGroup group)
    {
        if (_groups.ContainsKey(group.Id))
        {
            throw new InvalidOperationException($"Group with ID '{group.Id}' already exists.");
        }
        _groups[group.Id] = group;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TstGroup group)
    {
        if (!_groups.ContainsKey(group.Id))
        {
            throw new InvalidOperationException($"Group with ID '{group.Id}' not found.");
        }
        _groups[group.Id] = group;
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

