using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.INM;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Models.INM.DTO;
using GrpcServer.Infrastructure.Services.Generic;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.INM;

[ApiController]
[Route("/api/v1/inm/users")]
[Produces("application/json")]
public class InmUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public InmUsersController([FromKeyedServices(AppCode.INM)] IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// List all INM users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InmUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InmUserResponseDto>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users.Cast<InmUser>().Select(InmUserMapper.ToResponseDto));
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

        return Ok(InmUserMapper.ToResponseDto((InmUser)user));
    }

    /// <summary>
    /// Create a new INM user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InmUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InmUserResponseDto>> CreateUser([FromBody] InmUserRequestDto dto)
    {
        var user = InmUserMapper.FromRequestDto(dto);
        await _userService.AddAsync(user);

        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, InmUserMapper.ToResponseDto(user));
    }

    /// <summary>
    /// Replace an existing INM user
    /// </summary>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(InmUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InmUserResponseDto>> ReplaceUser(int userId, [FromBody] InmUserRequestDto dto)
    {
        var existingUser = await _userService.GetByIdAsync(userId);
        if (existingUser == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        var user = InmUserMapper.FromRequestDto(dto);
        user.Id = userId;
        await _userService.UpdateAsync(user);

        return Ok(InmUserMapper.ToResponseDto(user));
    }

    /// <summary>
    /// Partially update an INM user
    /// </summary>
    [HttpPatch("{userId}")]
    [ProducesResponseType(typeof(InmUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InmUserResponseDto>> PatchUser(int userId, [FromBody] InmUserRequestDto dto)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        InmUserMapper.ApplyPatch((InmUser)user, dto);
        await _userService.UpdateAsync(user);

        return Ok(InmUserMapper.ToResponseDto((InmUser)user));
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

