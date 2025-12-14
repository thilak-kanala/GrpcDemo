using GrpcServer.Infrastructure.DTOs;
using GrpcServer.Infrastructure.Mappers;
using GrpcServer.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers;

[ApiController]
[Produces("application/json")]
public class UserGroupRelationsController(IUserGroupRelationService relationService) : ControllerBase
{
    /// <summary>
    /// Get all groups that a user belongs to
    /// </summary>
    [HttpGet("/api/v1/users/{userId}/groups")]
    [ProducesResponseType(typeof(IEnumerable<GroupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetUserGroups(int userId)
    {
        var groups = await relationService.GetUserGroupsAsync(userId);
        return Ok(groups.Select(GroupMapper.ToDto));
    }

    /// <summary>
    /// Add a user to one or more groups
    /// </summary>
    [HttpPost("/api/v1/users/{userId}/groups")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddUserToGroups(int userId, [FromBody] AddUserToGroupsDto dto)
    {
        if (dto.GroupIds == null || dto.GroupIds.Count == 0)
            return BadRequest(new { message = "GroupIds list cannot be empty" });

        await relationService.AddUserToGroupsAsync(userId, dto.GroupIds);
        return NoContent();
    }

    /// <summary>
    /// Remove a user from a specific group
    /// </summary>
    [HttpDelete("/api/v1/users/{userId}/groups/{groupId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RemoveUserFromGroup(int userId, int groupId)
    {
        await relationService.RemoveUserFromGroupAsync(userId, groupId);
        return NoContent();
    }

    /// <summary>
    /// Get all users in a specific group
    /// </summary>
    [HttpGet("/api/v1/groups/{groupId}/users")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetGroupUsers(int groupId)
    {
        var users = await relationService.GetGroupUsersAsync(groupId);
        return Ok(users.Select(UserMapper.ToDto));
    }

    /// <summary>
    /// Add one or more users to a group
    /// </summary>
    [HttpPost("/api/v1/groups/{groupId}/users")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddUsersToGroup(int groupId, [FromBody] AddUsersToGroupDto dto)
    {
        if (dto.UserIds.Count == 0)
            return BadRequest(new { message = "UserIds list cannot be empty" });

        await relationService.AddUsersToGroupAsync(groupId, dto.UserIds);
        return NoContent();
    }

    /// <summary>
    /// Remove a specific user from a group (group context)
    /// </summary>
    [HttpDelete("/api/v1/groups/{groupId}/users/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RemoveUserFromGroupInGroupContext(int groupId, int userId)
    {
        await relationService.RemoveUserFromGroupInGroupContextAsync(groupId, userId);
        return NoContent();
    }
}

