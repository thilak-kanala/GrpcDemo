using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Services.TST;

/// <summary>
/// TST-specific implementation of IUserService with basic business logic for demonstration.
/// </summary>
public class TstUserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserValidator _userValidator;

    public TstUserService(IUserRepository userRepository, IUserValidator userValidator)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
    }

    public async Task<IBaseUser?> GetByIdAsync(string id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<IBaseUser>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task AddAsync(IBaseUser baseUser)
    {
        if (baseUser is not TstUser tstUser)
        {
            throw new ArgumentException("Only TstUser instances are supported by this service.", nameof(baseUser));
        }
        
        // Validate using the validator
        if (!_userValidator.IsValid(tstUser))
        {
            throw new ArgumentException("User validation failed. Ensure all required fields are populated.", nameof(baseUser));
        }
        
        // Simple business logic: Normalize email to lowercase
        tstUser.Email = tstUser.Email.ToLower();
        
        await _userRepository.AddAsync(tstUser);
    }

    public async Task UpdateAsync(IBaseUser baseUser)
    {
        if (baseUser is not TstUser tstUser)
        {
            throw new ArgumentException("Only TstUser instances are supported by this service.", nameof(baseUser));
        }
        
        // Validate using the validator
        if (!_userValidator.IsValid(tstUser))
        {
            throw new ArgumentException("User validation failed. Ensure all required fields are populated.", nameof(baseUser));
        }
        
        // Simple business logic: Normalize email to lowercase
        tstUser.Email = tstUser.Email.ToLower();
        
        await _userRepository.UpdateAsync(tstUser);
    }

    public async Task DeleteAsync(string id)
    {
        await _userRepository.DeleteAsync(id);
    }
}

