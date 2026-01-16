using GrpcServer.Infrastructure.DTO.TST;
using Microsoft.AspNetCore.Mvc;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Enum;

namespace GrpcServer.Infrastructure.Controllers.TST;

/// <summary>
/// REST API controller for managing TST Users.
/// Provides CRUD operations for user entities including retrieval, creation, updating, and deletion.
/// </summary>
/// <remarks>
/// This controller handles user-specific operations for the TST system.
/// All operations use DTOs for request/response mapping to ensure proper separation of concerns.
/// Business logic and validation are delegated to the TstUserService layer.
/// </remarks>
[ApiController]
[Route("api/v1/tst/users")]
[Produces("application/json")]
public class TstUserController : ControllerBase
{
    private readonly IUserService<TstUser> _userService;
    private readonly TstMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the TstUserController with required dependencies.
    /// </summary>
    /// <param name="userService">Service layer for user business logic</param>
    /// <param name="mapper">Mapper for converting between domain models and DTOs</param>
    public TstUserController(
        [FromKeyedServices(AppCode.TST)] IUserService<TstUser> userService, 
        [FromKeyedServices(AppCode.TST)] TstMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all users from the TST system.
    /// </summary>
    /// <returns>A list of all user response DTOs</returns>
    /// <response code="200">Successfully retrieved all users</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TstUserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TstUserResponseDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            var responseDtos = users.Select(_mapper.ToResponseDto);
            return Ok(responseDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving users.", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>The user response DTO if found</returns>
    /// <response code="200">Successfully retrieved the user</response>
    /// <response code="404">User with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TstUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TstUserResponseDto>> GetUserById(string id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
            {
                return NotFound(new { message = $"User with ID '{id}' not found." });
            }

            return Ok(_mapper.ToResponseDto(user));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving the user.", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new user in the TST system.
    /// </summary>
    /// <param name="requestDto">The user data to create</param>
    /// <returns>The created user response DTO</returns>
    /// <response code="201">User successfully created</response>
    /// <response code="400">Invalid user data provided or validation failed</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/tst/users
    ///     {
    ///         "id": "user123",
    ///         "userName": "john.doe",
    ///         "email": "john.doe@example.com",
    ///         "tstUserExtension1": "value1",
    ///         "tstUserExtension2": "value2"
    ///     }
    /// 
    /// Validation rules:
    /// - Id must not be empty
    /// - UserName must not be empty
    /// - Email must contain '@'
    /// - TstUserExtension1 must not be 'forbidden'
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(TstUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TstUserResponseDto>> CreateUser([FromBody] TstUserRequestDto requestDto)
    {
        try
        {
            // Map DTO to domain model
            var user = _mapper.FromRequestDto(requestDto);
            
            // Add user through service (includes validation and business logic)
            await _userService.AddAsync(user);
            
            // Return created user with location header
            var responseDto = _mapper.ToResponseDto(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, responseDto);
        }
        catch (ArgumentException ex)
        {
            // Validation errors from service layer
            return BadRequest(new { message = "User validation failed.", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while creating the user.", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing user in the TST system.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update</param>
    /// <param name="requestDto">The updated user data</param>
    /// <returns>No content on success</returns>
    /// <response code="204">User successfully updated</response>
    /// <response code="400">Invalid user data provided, validation failed, or ID mismatch</response>
    /// <response code="404">User with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/tst/users/user123
    ///     {
    ///         "id": "user123",
    ///         "userName": "john.doe.updated",
    ///         "email": "john.doe.updated@example.com",
    ///         "tstUserExtension1": "newValue1",
    ///         "tstUserExtension2": "newValue2"
    ///     }
    /// 
    /// Note: The ID in the URL must match the ID in the request body.
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] TstUserRequestDto requestDto)
    {
        try
        {
            // Validate ID consistency
            if (id != requestDto.BaseUser.Id)
            {
                return BadRequest(new { message = "ID in URL does not match ID in request body." });
            }

            // Check if user exists
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = $"User with ID '{id}' not found." });
            }

            // Map DTO to domain model
            var user = _mapper.FromRequestDto(requestDto);
            
            // Update user through service (includes validation and business logic)
            await _userService.UpdateAsync(user);
            
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            // Validation errors from service layer
            return BadRequest(new { message = "User validation failed.", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while updating the user.", error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a user from the TST system.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">User successfully deleted</response>
    /// <response code="404">User with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Warning: This operation permanently removes the user from the system.
    /// Consider the impact on user-group relationships before deletion.
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            // Check if user exists
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = $"User with ID '{id}' not found." });
            }

            // Delete user through service
            await _userService.DeleteAsync(id);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while deleting the user.", error = ex.Message });
        }
    }
}

