using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;

namespace GrpcServer.Infrastructure.Repositories.ABC;

public class AbcGroupRepository : IGroupRepository
{
    private readonly List<AbcGroup> _groups;
    private int _nextId;

    public AbcGroupRepository()
    {
        _groups = new List<AbcGroup>
        {
            new AbcGroup { Id = 1, DisplayName = "ABC Engineering", TenantId = "tenant-001" },
            new AbcGroup { Id = 2, DisplayName = "ABC Marketing", TenantId = "tenant-001" },
            new AbcGroup { Id = 3, DisplayName = "ABC Sales", TenantId = "tenant-002" },
            new AbcGroup { Id = 4, DisplayName = "ABC HR", TenantId = "tenant-001" },
            new AbcGroup { Id = 5, DisplayName = "ABC Finance", TenantId = "tenant-002" }
        };
        _nextId = 6;
    }

    public Task<IBaseGroup?> GetByIdAsync(int id)
    {
        var group = _groups.FirstOrDefault(g => g.Id == id);
        return Task.FromResult<IBaseGroup?>(group);
    }

    public Task<IEnumerable<IBaseGroup>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<IBaseGroup>>(_groups);
    }

    public Task AddAsync(IBaseGroup baseGroup)
    {
        if (baseGroup is not AbcGroup group)
        {
            throw new ArgumentException("Group must be of type AbcGroup", nameof(baseGroup));
        }

        if (group.Id == 0)
        {
            group.Id = _nextId++;
        }

        _groups.Add(group);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IBaseGroup baseGroup)
    {
        if (baseGroup is not AbcGroup group)
        {
            throw new ArgumentException("Group must be of type AbcGroup", nameof(baseGroup));
        }

        var existingGroup = _groups.FirstOrDefault(g => g.Id == group.Id);
        if (existingGroup == null)
        {
            throw new InvalidOperationException($"Group with ID {group.Id} not found");
        }

        existingGroup.DisplayName = group.DisplayName;
        existingGroup.TenantId = group.TenantId;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var group = _groups.FirstOrDefault(g => g.Id == id);
        if (group != null)
        {
            _groups.Remove(group);
        }

        return Task.CompletedTask;
    }
}

