using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Models.INM.DTO;
using GrpcServer.Infrastructure.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.INM;

[ApiController]
[Route("/api/v1/inm/user-group-relations")]
[Produces("application/json")]
public class InmUserGroupRelationsController : ControllerBase
{
    private readonly IUserGroupRelationService _relationService;
    private readonly IMapper<InmUser, InmUserRequestDto, InmUserResponseDto> _userMapper;
    private readonly IMapper<InmGroup, InmGroupRequestDto, InmGroupResponseDto> _groupMapper;

    public InmUserGroupRelationsController(
        [FromKeyedServices(AppCode.INM)] IUserGroupRelationService relationService,
        [FromKeyedServices(AppCode.INM)] IMapper<InmUser, InmUserRequestDto, InmUserResponseDto> userMapper,
        [FromKeyedServices(AppCode.INM)] IMapper<InmGroup, InmGroupRequestDto, InmGroupResponseDto> groupMapper)
    {
        _relationService = relationService;
        _userMapper = userMapper;
        _groupMapper = groupMapper;
    }

    /// <summary>
    /// Get all groups for a specific INM user
    /// </summary>
    [HttpGet("users/{userId}/groups")]
    [ProducesResponseType(typeof(IEnumerable<InmGroupResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<InmGroupResponseDto>>> GetUserGroups(int userId)
    {
        try
        {
            var groups = await _relationService.GetUserGroupsAsync(userId);
            return Ok(groups.Cast<InmGroup>().Select(_groupMapper.ToResponseDto));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Add an INM user to multiple groups
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
    /// Remove an INM user from a group
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
    /// Get all users in a specific INM group
    /// </summary>
    [HttpGet("groups/{groupId}/users")]
    [ProducesResponseType(typeof(IEnumerable<InmUserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<InmUserResponseDto>>> GetGroupUsers(int groupId)
    {
        try
        {
            var users = await _relationService.GetGroupUsersAsync(groupId);
            return Ok(users.Cast<InmUser>().Select(_userMapper.ToResponseDto));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Add multiple INM users to a group
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

