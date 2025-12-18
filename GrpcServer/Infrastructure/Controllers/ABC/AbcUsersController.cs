using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.ABC.DTO;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Validators.Common;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.ABC;

[ApiController]
[Route("/api/v1/abc/users")]
[Produces("application/json")]
public class AbcUsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> _mapper;
    private readonly IUserValidator _validator;

    public AbcUsersController(
        [FromKeyedServices(AppCode.ABC)] IUserService userService,
        [FromKeyedServices(AppCode.ABC)] IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> mapper,
        [FromKeyedServices(AppCode.ABC)] IUserValidator validator)
    {
        _userService = userService;
        _mapper = mapper;
        _validator = validator;
    }

    /// <summary>
    /// List all ABC users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AbcUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AbcUserResponseDto>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users.Cast<AbcUser>().Select(_mapper.ToResponseDto));
    }

    /// <summary>
    /// Get a specific ABC user by ID
    /// </summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(AbcUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbcUserResponseDto>> GetUser(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        return Ok(_mapper.ToResponseDto((AbcUser)user));
    }

    /// <summary>
    /// Create a new ABC user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AbcUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AbcUserResponseDto>> CreateUser([FromBody] AbcUserRequestDto dto)
    {
        var user = _mapper.FromRequestDto(dto);
        
        if (!_validator.IsValid(user))
            return BadRequest(new { message = "Invalid user data" });
        
        await _userService.AddAsync(user);

        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, _mapper.ToResponseDto(user));
    }

    /// <summary>
    /// Replace an existing ABC user
    /// </summary>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(AbcUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AbcUserResponseDto>> ReplaceUser(int userId, [FromBody] AbcUserRequestDto dto)
    {
        var existingUser = await _userService.GetByIdAsync(userId);
        if (existingUser == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        var user = _mapper.FromRequestDto(dto);
        user.Id = userId;
        
        if (!_validator.IsValid(user))
            return BadRequest(new { message = "Invalid user data" });
        
        await _userService.UpdateAsync(user);

        return Ok(_mapper.ToResponseDto(user));
    }

    /// <summary>
    /// Partially update an ABC user
    /// </summary>
    [HttpPatch("{userId}")]
    [ProducesResponseType(typeof(AbcUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AbcUserResponseDto>> PatchUser(int userId, [FromBody] AbcUserRequestDto dto)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        _mapper.ApplyPatch((AbcUser)user, dto);
        
        if (!_validator.IsValid(user))
            return BadRequest(new { message = "Invalid user data" });
        
        await _userService.UpdateAsync(user);

        return Ok(_mapper.ToResponseDto((AbcUser)user));
    }

    /// <summary>
    /// Delete an ABC user
    /// </summary>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        await _userService.DeleteAsync(userId);
        return NoContent();
    }
}

