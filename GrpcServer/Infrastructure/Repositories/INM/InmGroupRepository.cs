using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Repositories.Common;

namespace GrpcServer.Infrastructure.Repositories.INM;

public class InmGroupRepository : IGroupRepository
{
    private readonly List<InmGroup> _groups;
    private int _nextId;

    public InmGroupRepository()
    {
        _groups = new List<InmGroup>
        {
            new InmGroup { Id = 1, DisplayName = "Engineering", InmHost = "dev-server-01" },
            new InmGroup { Id = 2, DisplayName = "Marketing", InmHost = "dev-server-01" },
            new InmGroup { Id = 3, DisplayName = "Sales", InmHost = "dev-server-02" },
            new InmGroup { Id = 4, DisplayName = "Human Resources", InmHost = "dev-server-01" },
            new InmGroup { Id = 5, DisplayName = "Finance", InmHost = "dev-server-02" },
            new InmGroup { Id = 6, DisplayName = "Operations", InmHost = "dev-server-03" },
            new InmGroup { Id = 7, DisplayName = "Product Management", InmHost = "dev-server-01" },
            new InmGroup { Id = 8, DisplayName = "Customer Support", InmHost = "dev-server-02" },
            new InmGroup { Id = 9, DisplayName = "Research & Development", InmHost = "dev-server-03" },
            new InmGroup { Id = 10, DisplayName = "Quality Assurance", InmHost = "dev-server-01" },
            new InmGroup { Id = 11, DisplayName = "DevOps", InmHost = "dev-server-02" },
            new InmGroup { Id = 12, DisplayName = "Security", InmHost = "dev-server-03" }
        };
        _nextId = 13;
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
        if (baseGroup is not InmGroup group)
        {
            throw new ArgumentException("Group must be of type InmGroup", nameof(baseGroup));
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
        if (baseGroup is not InmGroup group)
        {
            throw new ArgumentException("Group must be of type InmGroup", nameof(baseGroup));
        }

        var existingGroup = _groups.FirstOrDefault(g => g.Id == group.Id);
        if (existingGroup == null)
        {
            throw new InvalidOperationException($"Group with ID {group.Id} not found");
        }

        existingGroup.DisplayName = group.DisplayName;
        existingGroup.InmHost = group.InmHost;

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

