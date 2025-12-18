using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.ABC.DTO;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.ABC;

[ApiController]
[Route("/api/v1/abc/user-group-relations")]
[Produces("application/json")]
public class AbcUserGroupRelationsController : ControllerBase
{
    private readonly IUserGroupRelationService _relationService;
    private readonly IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> _userMapper;
    private readonly IMapper<AbcGroup, AbcGroupRequestDto, AbcGroupResponseDto> _groupMapper;

    public AbcUserGroupRelationsController(
        [FromKeyedServices(AppCode.ABC)] IUserGroupRelationService relationService,
        [FromKeyedServices(AppCode.ABC)] IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> userMapper,
        [FromKeyedServices(AppCode.ABC)] IMapper<AbcGroup, AbcGroupRequestDto, AbcGroupResponseDto> groupMapper)
    {
        _relationService = relationService;
        _userMapper = userMapper;
        _groupMapper = groupMapper;
    }

    /// <summary>
    /// Get all groups for a specific ABC user
    /// </summary>
    [HttpGet("users/{userId}/groups")]
    [ProducesResponseType(typeof(IEnumerable<AbcGroupResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<AbcGroupResponseDto>>> GetUserGroups(int userId)
    {
        try
        {
            var groups = await _relationService.GetUserGroupsAsync(userId);
            return Ok(groups.Cast<AbcGroup>().Select(_groupMapper.ToResponseDto));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Add an ABC user to multiple groups
    /// </summary>
    [HttpPost("users/{userId}/groups")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddUserToGroups(int userId, [FromBody] RelationDtos.AddUserToGroupsRequestDto dto)
    {
        try
        {
            await _relationService.AddUserToGroupsAsync(userId, dto.GroupIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove an ABC user from a group
    /// </summary>
    [HttpDelete("users/{userId}/groups/{groupId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveUserFromGroup(int userId, int groupId)
    {
        try
        {
            await _relationService.RemoveUserFromGroupAsync(userId, groupId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all users in a specific ABC group
    /// </summary>
    [HttpGet("groups/{groupId}/users")]
    [ProducesResponseType(typeof(IEnumerable<AbcUserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<AbcUserResponseDto>>> GetGroupUsers(int groupId)
    {
        try
        {
            var users = await _relationService.GetGroupUsersAsync(groupId);
            return Ok(users.Cast<AbcUser>().Select(_userMapper.ToResponseDto));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Add multiple ABC users to a group
    /// </summary>
    [HttpPost("groups/{groupId}/users")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddUsersToGroup(int groupId, [FromBody] RelationDtos.AddUsersToGroupRequestDto dto)
    {
        try
        {
            await _relationService.AddUsersToGroupAsync(groupId, dto.UserIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

