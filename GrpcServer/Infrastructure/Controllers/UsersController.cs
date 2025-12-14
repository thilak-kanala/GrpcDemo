using GrpcServer.Infrastructure.DTOs;
using GrpcServer.Infrastructure.Mappers;
using GrpcServer.Infrastructure.Services;
using GrpcServer.Infrastructure.Validators;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers;

[ApiController]
[Route("/api/v1/users")]
[Produces("application/json")]
public class UsersController(UserService userService) : ControllerBase
{
    /// <summary>
    /// List all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await userService.GetAllAsync();
        return Ok(users.Select(UserMapper.ToDto));
    }

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(int userId)
    {
        var user = await userService.GetByIdAsync(userId);
        
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        return Ok(UserMapper.ToDto(user));
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDto dto)
    {
        var validation = UserValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var user = UserMapper.ToEntity(dto);
        await userService.AddAsync(user);

        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, UserMapper.ToDto(user));
    }

    /// <summary>
    /// Replace an existing user
    /// </summary>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> ReplaceUser(int userId, UserDto dto)
    {
        var validation = UserValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var existingUser = await userService.GetByIdAsync(userId);
        if (existingUser == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        var user = UserMapper.ToEntity(dto);
        await userService.UpdateAsync(user);

        return Ok(UserMapper.ToDto(user));
    }

    /// <summary>
    /// Partially update a user
    /// </summary>
    [HttpPatch("{userId}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> PatchUser(int userId, [FromBody] UserDto dto)
    {
        var validation = UserValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var user = await userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        UserMapper.ApplyPatch(user, dto);
        await userService.UpdateAsync(user);

        return Ok(UserMapper.ToDto(user));
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(int userId)
    {
        var user = await userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        await userService.DeleteAsync(userId);
        return NoContent();
    }
}

