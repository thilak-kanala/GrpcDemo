using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Services.TST;

/// <summary>
/// TST-specific implementation of IGroupService with basic business logic for demonstration.
/// </summary>
public class TstGroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupValidator _groupValidator;

    public TstGroupService(IGroupRepository groupRepository, IGroupValidator groupValidator)
    {
        _groupRepository = groupRepository;
        _groupValidator = groupValidator;
    }

    public async Task<IBaseGroup?> GetByIdAsync(string id)
    {
        return await _groupRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<IBaseGroup>> GetAllAsync()
    {
        return await _groupRepository.GetAllAsync();
    }

    public async Task AddAsync(IBaseGroup baseGroup)
    {
        if (baseGroup is not TstGroup tstGroup)
        {
            throw new ArgumentException("Only TstGroup instances are supported by this service.", nameof(baseGroup));
        }
        
        // Validate using the validator
        if (!_groupValidator.IsValid(tstGroup))
        {
            throw new ArgumentException("Group validation failed. Ensure all required fields are populated.", nameof(baseGroup));
        }
        
        // Simple business logic: Trim whitespace from display name
        tstGroup.DisplayName = tstGroup.DisplayName.Trim();
        
        await _groupRepository.AddAsync(tstGroup);
    }

    public async Task UpdateAsync(IBaseGroup baseGroup)
    {
        if (baseGroup is not TstGroup tstGroup)
        {
            throw new ArgumentException("Only TstGroup instances are supported by this service.", nameof(baseGroup));
        }
        
        // Validate using the validator
        if (!_groupValidator.IsValid(tstGroup))
        {
            throw new ArgumentException("Group validation failed. Ensure all required fields are populated.", nameof(baseGroup));
        }
        
        // Simple business logic: Trim whitespace from display name
        tstGroup.DisplayName = tstGroup.DisplayName.Trim();
        
        await _groupRepository.UpdateAsync(tstGroup);
    }

    public async Task DeleteAsync(string id)
    {
        await _groupRepository.DeleteAsync(id);
    }
}

