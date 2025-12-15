using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.ABC;
using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.ABC.DTO;
using GrpcServer.Infrastructure.Services.Generic;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.ABC;

[ApiController]
[Route("/api/v1/abc/users")]
[Produces("application/json")]
public class AbcUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AbcUsersController([FromKeyedServices(AppCode.ABC)] IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// List all ABC users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AbcUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AbcUserResponseDto>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users.Cast<AbcUser>().Select(AbcUserMapper.ToResponseDto));
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

        return Ok(AbcUserMapper.ToResponseDto((AbcUser)user));
    }

    /// <summary>
    /// Create a new ABC user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AbcUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AbcUserResponseDto>> CreateUser([FromBody] AbcUserRequestDto dto)
    {
        var user = AbcUserMapper.FromRequestDto(dto);
        await _userService.AddAsync(user);

        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, AbcUserMapper.ToResponseDto(user));
    }

    /// <summary>
    /// Replace an existing ABC user
    /// </summary>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(AbcUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbcUserResponseDto>> ReplaceUser(int userId, [FromBody] AbcUserRequestDto dto)
    {
        var existingUser = await _userService.GetByIdAsync(userId);
        if (existingUser == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        var user = AbcUserMapper.FromRequestDto(dto);
        user.Id = userId;
        await _userService.UpdateAsync(user);

        return Ok(AbcUserMapper.ToResponseDto(user));
    }

    /// <summary>
    /// Partially update an ABC user
    /// </summary>
    [HttpPatch("{userId}")]
    [ProducesResponseType(typeof(AbcUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbcUserResponseDto>> PatchUser(int userId, [FromBody] AbcUserRequestDto dto)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        AbcUserMapper.ApplyPatch((AbcUser)user, dto);
        await _userService.UpdateAsync(user);

        return Ok(AbcUserMapper.ToResponseDto((AbcUser)user));
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

