using GrpcServer.Infrastructure.DTOs;
using GrpcServer.Infrastructure.Mappers;
using GrpcServer.Infrastructure.Services;
using GrpcServer.Infrastructure.Validators;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers;

[ApiController]
[Route("/api/v1/groups")]
[Produces("application/json")]
public class GroupsController(GroupService groupService) : ControllerBase
{
    /// <summary>
    /// List all groups
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GroupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups()
    {
        var groups = await groupService.GetAllAsync();
        return Ok(groups.Select(GroupMapper.ToDto));
    }

    /// <summary>
    /// Get a specific group by ID
    /// </summary>
    [HttpGet("{groupId}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> GetGroup(int groupId)
    {
        var group = await groupService.GetByIdAsync(groupId);
        
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        return Ok(GroupMapper.ToDto(group));
    }

    /// <summary>
    /// Create a new group
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupDto>> CreateGroup([FromBody] GroupDto dto)
    {
        var validation = GroupValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var group = GroupMapper.ToEntity(dto);
        await groupService.AddAsync(group);

        return CreatedAtAction(nameof(GetGroup), new { groupId = group.Id }, GroupMapper.ToDto(group));
    }

    /// <summary>
    /// Replace an existing group
    /// </summary>
    [HttpPut("{groupId}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> ReplaceGroup(int groupId, [FromBody] GroupDto dto)
    {
        var validation = GroupValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var existingGroup = await groupService.GetByIdAsync(groupId);
        if (existingGroup == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        var group = GroupMapper.ToEntity(dto);
        await groupService.UpdateAsync(group);

        return Ok(GroupMapper.ToDto(group));
    }

    /// <summary>
    /// Partially update a group
    /// </summary>
    [HttpPatch("{groupId}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> PatchGroup(int groupId, [FromBody] GroupDto dto)
    {
        var validation = GroupValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var group = await groupService.GetByIdAsync(groupId);
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        GroupMapper.ApplyPatch(group, dto);
        await groupService.UpdateAsync(group);

        return Ok(GroupMapper.ToDto(group));
    }

    /// <summary>
    /// Delete a group
    /// </summary>
    [HttpDelete("{groupId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGroup(int groupId)
    {
        var group = await groupService.GetByIdAsync(groupId);
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        await groupService.DeleteAsync(groupId);
        return NoContent();
    }
}

