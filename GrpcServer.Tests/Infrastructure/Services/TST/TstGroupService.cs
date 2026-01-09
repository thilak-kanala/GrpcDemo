using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Services.TST;

/// <summary>
/// TST-specific implementation of IGroupService with basic business logic for demonstration.
/// </summary>
public class TstGroupService : IGroupService<TstGroup>
{
    private readonly IGroupRepository<TstGroup> _groupRepository;
    private readonly IGroupValidator _groupValidator;

    public TstGroupService(IGroupRepository<TstGroup> groupRepository, IGroupValidator groupValidator)
    {
        _groupRepository = groupRepository;
        _groupValidator = groupValidator;
    }

    public async Task<TstGroup?> GetByIdAsync(string id)
    {
        return await _groupRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TstGroup>> GetAllAsync()
    {
        return await _groupRepository.GetAllAsync();
    }

    public async Task AddAsync(TstGroup group)
    {
        // Validate using the validator
        if (!_groupValidator.IsValid(group))
        {
            throw new ArgumentException("Group validation failed. Ensure all required fields are populated.", nameof(group));
        }
        
        // Simple business logic: Trim whitespace from display name
        group.DisplayName = group.DisplayName.Trim();
        
        await _groupRepository.AddAsync(group);
    }

    public async Task UpdateAsync(TstGroup group)
    {
        // Validate using the validator
        if (!_groupValidator.IsValid(group))
        {
            throw new ArgumentException("Group validation failed. Ensure all required fields are populated.", nameof(group));
        }
        
        // Simple business logic: Trim whitespace from display name
        group.DisplayName = group.DisplayName.Trim();
        
        await _groupRepository.UpdateAsync(group);
    }

    public async Task DeleteAsync(string id)
    {
        await _groupRepository.DeleteAsync(id);
    }
}

