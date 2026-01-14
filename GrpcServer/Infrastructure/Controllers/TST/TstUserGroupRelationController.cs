using GrpcServer.Infrastructure.DTO.TST;
using Microsoft.AspNetCore.Mvc;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Enum;
using Microsoft.Extensions.DependencyInjection;
using static GrpcServer.Infrastructure.Models.Common.RelationDtos;

namespace GrpcServer.Infrastructure.Controllers.TST;

/// <summary>
/// REST API controller for managing TST User-Group relationships.
/// Provides operations to manage many-to-many relationships between users and groups.
/// </summary>
/// <remarks>
/// This controller handles the association between users and groups in the TST system.
/// It supports operations to:
/// - Retrieve users belonging to a group
/// - Retrieve groups a user belongs to
/// - Add users to groups (both individual and bulk operations)
/// - Remove users from groups
/// All entity existence is validated before performing relationship operations.
/// </remarks>
[ApiController]
[Route("api/v1/tst")]
[Produces("application/json")]
public class TstUserGroupRelationController : ControllerBase
{
    private readonly IUserGroupRelationService<TstUser, TstGroup> _relationService;
    private readonly TstMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the TstUserGroupRelationController with required dependencies.
    /// </summary>
    /// <param name="relationService">Service layer for user-group relationship business logic</param>
    /// <param name="mapper">Mapper for converting between domain models and DTOs</param>
    public TstUserGroupRelationController(
        [FromKeyedServices(AppCode.TST)] IUserGroupRelationService<TstUser, TstGroup> relationService,
        [FromKeyedServices(AppCode.TST)] TstMapper mapper)
    {
        _relationService = relationService;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all groups that a specific user belongs to.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>A list of group response DTOs that the user belongs to</returns>
    /// <response code="200">Successfully retrieved user's groups</response>
    /// <response code="404">User with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// This endpoint returns all groups associated with the specified user.
    /// An empty list is returned if the user exists but belongs to no groups.
    /// </remarks>
    [HttpGet("users/{userId}/groups")]
    [ProducesResponseType(typeof(IEnumerable<TstGroupResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TstGroupResponseDto>>> GetUserGroups(string userId)
    {
        try
        {
            var groups = await _relationService.GetUserGroupsAsync(userId);
            var responseDtos = groups.Select(_mapper.ToResponseDto);
            return Ok(responseDtos);
        }
        catch (InvalidOperationException ex)
        {
            // User not found or other business logic error
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving user groups.", error = ex.Message });
        }
    }

    /// <summary>
    /// Adds a user to multiple groups.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="request">Request containing list of group IDs to add the user to</param>
    /// <returns>No content on success</returns>
    /// <response code="204">User successfully added to all specified groups</response>
    /// <response code="400">Invalid request data provided</response>
    /// <response code="404">User or one of the specified groups was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/tst/users/user123/groups
    ///     {
    ///         "groupIds": ["group1", "group2", "group3"]
    ///     }
    /// 
    /// This operation validates:
    /// - The user exists
    /// - All specified groups exist
    /// - Adds the user to each group atomically
    /// 
    /// Note: If any group ID is invalid, the entire operation fails.
    /// </remarks>
    [HttpPost("users/{userId}/groups")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUserToGroups(
        string userId, 
        [FromBody] AddUserToGroupsRequestDto request)
    {
        try
        {
            if (!request.GroupIds.Any())
            {
                return BadRequest(new { message = "GroupIds list cannot be empty." });
            }

            await _relationService.AddUserToGroupsAsync(userId, request.GroupIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // User or group not found
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while adding user to groups.", error = ex.Message });
        }
    }

    /// <summary>
    /// Removes a user from a specific group.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="groupId">The unique identifier of the group</param>
    /// <returns>No content on success</returns>
    /// <response code="204">User successfully removed from the group</response>
    /// <response code="404">User or group with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// This operation:
    /// - Validates both user and group exist
    /// - Removes the association between them
    /// - Is idempotent (removing a non-existent relationship succeeds)
    /// </remarks>
    [HttpDelete("users/{userId}/groups/{groupId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveUserFromGroup(string userId, string groupId)
    {
        try
        {
            await _relationService.RemoveUserFromGroupAsync(userId, groupId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // User or group not found
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while removing user from group.", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all users that belong to a specific group.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group</param>
    /// <returns>A list of user response DTOs that belong to the group</returns>
    /// <response code="200">Successfully retrieved group's users</response>
    /// <response code="404">Group with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// This endpoint returns all users associated with the specified group.
    /// An empty list is returned if the group exists but contains no users.
    /// </remarks>
    [HttpGet("groups/{groupId}/users")]
    [ProducesResponseType(typeof(IEnumerable<TstUserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TstUserResponseDto>>> GetGroupUsers(string groupId)
    {
        try
        {
            var users = await _relationService.GetGroupUsersAsync(groupId);
            var responseDtos = users.Select(_mapper.ToResponseDto);
            return Ok(responseDtos);
        }
        catch (InvalidOperationException ex)
        {
            // Group not found or other business logic error
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving group users.", error = ex.Message });
        }
    }

    /// <summary>
    /// Adds multiple users to a specific group.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group</param>
    /// <param name="request">Request containing list of user IDs to add to the group</param>
    /// <returns>No content on success</returns>
    /// <response code="204">All users successfully added to the group</response>
    /// <response code="400">Invalid request data provided</response>
    /// <response code="404">Group or one of the specified users was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/tst/groups/group123/users
    ///     {
    ///         "userIds": ["user1", "user2", "user3"]
    ///     }
    /// 
    /// This operation validates:
    /// - The group exists
    /// - All specified users exist
    /// - Adds each user to the group atomically
    /// 
    /// Note: If any user ID is invalid, the entire operation fails.
    /// </remarks>
    [HttpPost("groups/{groupId}/users")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUsersToGroup(
        string groupId, 
        [FromBody] AddUsersToGroupRequestDto request)
    {
        try
        {
            if (!request.UserIds.Any())
            {
                return BadRequest(new { message = "UserIds list cannot be empty." });
            }

            await _relationService.AddUsersToGroupAsync(groupId, request.UserIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            // Group or user not found
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while adding users to group.", error = ex.Message });
        }
    }
}

