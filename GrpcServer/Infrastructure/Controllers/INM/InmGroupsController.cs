using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Models.INM.DTO;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Validators.Common;
using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers.INM;

[ApiController]
[Route("/api/v1/inm/groups")]
[Produces("application/json")]
public class InmGroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly IMapper<InmGroup, InmGroupRequestDto, InmGroupResponseDto> _mapper;
    private readonly IGroupValidator _validator;

    public InmGroupsController(
        [FromKeyedServices(AppCode.INM)] IGroupService groupService,
        [FromKeyedServices(AppCode.INM)] IMapper<InmGroup, InmGroupRequestDto, InmGroupResponseDto> mapper,
        [FromKeyedServices(AppCode.INM)] IGroupValidator validator)
    {
        _groupService = groupService;
        _mapper = mapper;
        _validator = validator;
    }

    /// <summary>
    /// List all INM groups
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InmGroupResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InmGroupResponseDto>>> GetGroups()
    {
        var groups = await _groupService.GetAllAsync();
        return Ok(groups.Cast<InmGroup>().Select(_mapper.ToResponseDto));
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

        return Ok(_mapper.ToResponseDto((InmGroup)group));
    }

    /// <summary>
    /// Create a new INM group
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InmGroupResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InmGroupResponseDto>> CreateGroup([FromBody] InmGroupRequestDto dto)
    {
        var group = _mapper.FromRequestDto(dto);
        
        if (!_validator.IsValid(group))
            return BadRequest(new { message = "Invalid group data" });
        
        await _groupService.AddAsync(group);

        return CreatedAtAction(nameof(GetGroup), new { groupId = group.Id }, _mapper.ToResponseDto(group));
    }

    /// <summary>
    /// Replace an existing INM group
    /// </summary>
    [HttpPut("{groupId}")]
    [ProducesResponseType(typeof(InmGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InmGroupResponseDto>> ReplaceGroup(int groupId, [FromBody] InmGroupRequestDto dto)
    {
        var existingGroup = await _groupService.GetByIdAsync(groupId);
        if (existingGroup == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        var group = _mapper.FromRequestDto(dto);
        group.Id = groupId;
        
        if (!_validator.IsValid(group))
            return BadRequest(new { message = "Invalid group data" });
        
        await _groupService.UpdateAsync(group);

        return Ok(_mapper.ToResponseDto(group));
    }

    /// <summary>
    /// Partially update an INM group
    /// </summary>
    [HttpPatch("{groupId}")]
    [ProducesResponseType(typeof(InmGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InmGroupResponseDto>> PatchGroup(int groupId, [FromBody] InmGroupRequestDto dto)
    {
        var group = await _groupService.GetByIdAsync(groupId);
        if (group == null)
            return NotFound(new { message = $"Group with ID {groupId} not found" });

        _mapper.ApplyPatch((InmGroup)group, dto);
        
        if (!_validator.IsValid(group))
            return BadRequest(new { message = "Invalid group data" });
        
        await _groupService.UpdateAsync(group);

        return Ok(_mapper.ToResponseDto((InmGroup)group));
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

