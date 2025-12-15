using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.INM;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Models.INM.DTO;
using GrpcServer.Infrastructure.Services.Generic;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.INM;

[ApiController]
[Route($"/api/v1/inm/groups")]
[Produces("application/json")]
public class InmGroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public InmGroupsController([FromKeyedServices(AppCode.INM)] IGroupService groupService)
    {
        _groupService = groupService;
    }

    /// <summary>
    /// List all INM groups
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InmGroupResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InmGroupResponseDto>>> GetGroups()
    {
        var groups = await _groupService.GetAllAsync();
        return Ok(groups.Cast<InmGroup>().Select(InmGroupMapper.ToResponseDto));
    }

    /// <summary>
    /// Get a specific INM group by ID
    /// </summary>
    [HttpGet("{groupId}")]
    [ProducesResponseType(typeof(InmGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InmGroupResponseDto>> GetGroup(int groupId)
    {
        var group = await _groupService.GetByIdAsync(groupId);
        
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        return Ok(InmGroupMapper.ToResponseDto((InmGroup)group));
    }

    /// <summary>
    /// Create a new INM group
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InmGroupResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InmGroupResponseDto>> CreateGroup([FromBody] InmGroupRequestDto dto)
    {
        var group = InmGroupMapper.FromRequestDto(dto);
        await _groupService.AddAsync(group);

        return CreatedAtAction(nameof(GetGroup), new { groupId = group.Id }, InmGroupMapper.ToResponseDto(group));
    }

    /// <summary>
    /// Replace an existing INM group
    /// </summary>
    [HttpPut("{groupId}")]
    [ProducesResponseType(typeof(InmGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InmGroupResponseDto>> ReplaceGroup(int groupId, [FromBody] InmGroupRequestDto dto)
    {
        var existingGroup = await _groupService.GetByIdAsync(groupId);
        if (existingGroup == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        var group = InmGroupMapper.FromRequestDto(dto);
        group.Id = groupId;
        await _groupService.UpdateAsync(group);

        return Ok(InmGroupMapper.ToResponseDto(group));
    }

    /// <summary>
    /// Partially update an INM group
    /// </summary>
    [HttpPatch("{groupId}")]
    [ProducesResponseType(typeof(InmGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InmGroupResponseDto>> PatchGroup(int groupId, [FromBody] InmGroupRequestDto dto)
    {
        var group = await _groupService.GetByIdAsync(groupId);
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        InmGroupMapper.ApplyPatch((InmGroup)group, dto);
        await _groupService.UpdateAsync(group);

        return Ok(InmGroupMapper.ToResponseDto((InmGroup)group));
    }

    /// <summary>
    /// Delete an INM group
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

