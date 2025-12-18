using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Models.INM.DTO;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Validators.Common;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.INM;

[ApiController]
[Route("/api/v1/inm/users")]
[Produces("application/json")]
public class InmUsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper<InmUser, InmUserRequestDto, InmUserResponseDto> _mapper;
    private readonly IUserValidator _validator;

    public InmUsersController(
        [FromKeyedServices(AppCode.INM)] IUserService userService,
        [FromKeyedServices(AppCode.INM)] IMapper<InmUser, InmUserRequestDto, InmUserResponseDto> mapper,
        [FromKeyedServices(AppCode.INM)] IUserValidator validator)
    {
        _userService = userService;
        _mapper = mapper;
        _validator = validator;
    }

    /// <summary>
    /// List all INM users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InmUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InmUserResponseDto>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users.Cast<InmUser>().Select(_mapper.ToResponseDto));
    }

    /// <summary>
    /// Get a specific INM user by ID
    /// </summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(InmUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InmUserResponseDto>> GetUser(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        return Ok(_mapper.ToResponseDto((InmUser)user));
    }

    /// <summary>
    /// Create a new INM user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InmUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InmUserResponseDto>> CreateUser([FromBody] InmUserRequestDto dto)
    {
        var user = _mapper.FromRequestDto(dto);
        
        if (!_validator.IsValid(user))
            return BadRequest(new { message = "Invalid user data" });
        
        await _userService.AddAsync(user);

        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, _mapper.ToResponseDto(user));
    }

    /// <summary>
    /// Replace an existing INM user
    /// </summary>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(InmUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InmUserResponseDto>> ReplaceUser(int userId, [FromBody] InmUserRequestDto dto)
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
    /// Partially update an INM user
    /// </summary>
    [HttpPatch("{userId}")]
    [ProducesResponseType(typeof(InmUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InmUserResponseDto>> PatchUser(int userId, [FromBody] InmUserRequestDto dto)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        _mapper.ApplyPatch((InmUser)user, dto);
        
        if (!_validator.IsValid(user))
            return BadRequest(new { message = "Invalid user data" });
        
        await _userService.UpdateAsync(user);

        return Ok(_mapper.ToResponseDto((InmUser)user));
    }

    /// <summary>
    /// Delete an INM user
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

