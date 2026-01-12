using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Infrastructure.Enum;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServer.Infrastructure.Services.TST;

/// <summary>
/// TST-specific implementation of IUserService with basic business logic for demonstration.
/// </summary>
public class TstUserService : IUserService<TstUser>
{
    private readonly IUserRepository<TstUser> _userRepository;
    private readonly IValidator<TstUser> _userValidator;

    public TstUserService(
        [FromKeyedServices(AppCode.TST)] IUserRepository<TstUser> userRepository, 
        [FromKeyedServices(AppCode.TST)] IValidator<TstUser> userValidator)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
    }

    public async Task<TstUser?> GetByIdAsync(string id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TstUser>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task AddAsync(TstUser user)
    {
        // Validate using the validator
        if (!_userValidator.IsValid(user))
        {
            throw new ArgumentException("User validation failed. Ensure all required fields are populated.", nameof(user));
        }
        
        // Simple business logic: Normalize email to lowercase
        user.Email = user.Email.ToLower();
        
        await _userRepository.AddAsync(user);
    }

    public async Task UpdateAsync(TstUser user)
    {
        // Validate using the validator
        if (!_userValidator.IsValid(user))
        {
            throw new ArgumentException("User validation failed. Ensure all required fields are populated.", nameof(user));
        }
        
        // Simple business logic: Normalize email to lowercase
        user.Email = user.Email.ToLower();
        
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(string id)
    {
        await _userRepository.DeleteAsync(id);
    }
}

