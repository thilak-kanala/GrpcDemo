using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.ABC;
using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.ABC.DTO;
using GrpcServer.Infrastructure.Services.Generic;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.ABC;

[ApiController]
[Route("/api/v1/abc/groups")]
[Produces("application/json")]
public class AbcGroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public AbcGroupsController([FromKeyedServices(AppCode.ABC)] IGroupService groupService)
    {
        _groupService = groupService;
    }

    /// <summary>
    /// List all ABC groups
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AbcGroupResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AbcGroupResponseDto>>> GetGroups()
    {
        var groups = await _groupService.GetAllAsync();
        return Ok(groups.Cast<AbcGroup>().Select(AbcGroupMapper.ToResponseDto));
    }

    /// <summary>
    /// Get a specific ABC group by ID
    /// </summary>
    [HttpGet("{groupId}")]
    [ProducesResponseType(typeof(AbcGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbcGroupResponseDto>> GetGroup(int groupId)
    {
        var group = await _groupService.GetByIdAsync(groupId);
        
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        return Ok(AbcGroupMapper.ToResponseDto((AbcGroup)group));
    }

    /// <summary>
    /// Create a new ABC group
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AbcGroupResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AbcGroupResponseDto>> CreateGroup([FromBody] AbcGroupRequestDto dto)
    {
        var group = AbcGroupMapper.FromRequestDto(dto);
        await _groupService.AddAsync(group);

        return CreatedAtAction(nameof(GetGroup), new { groupId = group.Id }, AbcGroupMapper.ToResponseDto(group));
    }

    /// <summary>
    /// Replace an existing ABC group
    /// </summary>
    [HttpPut("{groupId}")]
    [ProducesResponseType(typeof(AbcGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbcGroupResponseDto>> ReplaceGroup(int groupId, [FromBody] AbcGroupRequestDto dto)
    {
        var existingGroup = await _groupService.GetByIdAsync(groupId);
        if (existingGroup == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        var group = AbcGroupMapper.FromRequestDto(dto);
        group.Id = groupId;
        await _groupService.UpdateAsync(group);

        return Ok(AbcGroupMapper.ToResponseDto(group));
    }

    /// <summary>
    /// Partially update an ABC group
    /// </summary>
    [HttpPatch("{groupId}")]
    [ProducesResponseType(typeof(AbcGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbcGroupResponseDto>> PatchGroup(int groupId, [FromBody] AbcGroupRequestDto dto)
    {
        var group = await _groupService.GetByIdAsync(groupId);
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        AbcGroupMapper.ApplyPatch((AbcGroup)group, dto);
        await _groupService.UpdateAsync(group);

        return Ok(AbcGroupMapper.ToResponseDto((AbcGroup)group));
    }

    /// <summary>
    /// Delete an ABC group
    /// </summary>
    [HttpDelete("{groupId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGroup(int groupId)
    {
        var group = await _groupService.GetByIdAsync(groupId);
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        await _groupService.DeleteAsync(groupId);
        return NoContent();
    }
}

