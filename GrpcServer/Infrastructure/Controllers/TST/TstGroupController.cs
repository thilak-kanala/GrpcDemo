using Microsoft.AspNetCore.Mvc;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Models.TST.DTO;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Enum;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServer.Infrastructure.Controllers.TST;

/// <summary>
/// REST API controller for managing TST Groups.
/// Provides CRUD operations for group entities including retrieval, creation, updating, and deletion.
/// </summary>
/// <remarks>
/// This controller handles group-specific operations for the TST system.
/// All operations use DTOs for request/response mapping to ensure proper separation of concerns.
/// Business logic and validation are delegated to the TstGroupService layer.
/// </remarks>
[ApiController]
[Route("api/v1/tst/groups")]
[Produces("application/json")]
public class TstGroupController : ControllerBase
{
    private readonly IGroupService<TstGroup> _groupService;
    private readonly TstMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the TstGroupController with required dependencies.
    /// </summary>
    /// <param name="groupService">Service layer for group business logic</param>
    /// <param name="mapper">Mapper for converting between domain models and DTOs</param>
    public TstGroupController(
        [FromKeyedServices(AppCode.TST)] IGroupService<TstGroup> groupService, 
        [FromKeyedServices(AppCode.TST)] TstMapper mapper)
    {
        _groupService = groupService;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all groups from the TST system.
    /// </summary>
    /// <returns>A list of all group response DTOs</returns>
    /// <response code="200">Successfully retrieved all groups</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TstGroupResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TstGroupResponseDto>>> GetAllGroups()
    {
        try
        {
            var groups = await _groupService.GetAllAsync();
            var responseDtos = groups.Select(_mapper.ToResponseDto);
            return Ok(responseDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving groups.", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a specific group by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the group</param>
    /// <returns>The group response DTO if found</returns>
    /// <response code="200">Successfully retrieved the group</response>
    /// <response code="404">Group with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TstGroupResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TstGroupResponseDto>> GetGroupById(string id)
    {
        try
        {
            var group = await _groupService.GetByIdAsync(id);
            
            if (group == null)
            {
                return NotFound(new { message = $"Group with ID '{id}' not found." });
            }

            return Ok(_mapper.ToResponseDto(group));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving the group.", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new group in the TST system.
    /// </summary>
    /// <param name="requestDto">The group data to create</param>
    /// <returns>The created group response DTO</returns>
    /// <response code="201">Group successfully created</response>
    /// <response code="400">Invalid group data provided or validation failed</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/tst/groups
    ///     {
    ///         "id": "group123",
    ///         "displayName": "Engineering Team",
    ///         "tstGroupExtension1": "value1",
    ///         "tstGroupExtension2": "value2"
    ///     }
    /// 
    /// Validation rules:
    /// - Id must not be empty
    /// - DisplayName must not be empty
    /// - TstGroupExtension1 is required and minimum 5 characters
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(TstGroupResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TstGroupResponseDto>> CreateGroup([FromBody] TstGroupRequestDto requestDto)
    {
        try
        {
            // Map DTO to domain model
            var group = _mapper.FromRequestDto(requestDto);
            
            // Add group through service (includes validation and business logic)
            await _groupService.AddAsync(group);
            
            // Return created group with location header
            var responseDto = _mapper.ToResponseDto(group);
            return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, responseDto);
        }
        catch (ArgumentException ex)
        {
            // Validation errors from service layer
            return BadRequest(new { message = "Group validation failed.", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while creating the group.", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing group in the TST system.
    /// </summary>
    /// <param name="id">The unique identifier of the group to update</param>
    /// <param name="requestDto">The updated group data</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Group successfully updated</response>
    /// <response code="400">Invalid group data provided, validation failed, or ID mismatch</response>
    /// <response code="404">Group with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/tst/groups/group123
    ///     {
    ///         "id": "group123",
    ///         "displayName": "Engineering Team - Updated",
    ///         "tstGroupExtension1": "department-eng-updated",
    ///         "tstGroupExtension2": "floor-4"
    ///     }
    /// 
    /// Note: The ID in the URL must match the ID in the request body.
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateGroup(string id, [FromBody] TstGroupRequestDto requestDto)
    {
        try
        {
            // Validate ID consistency
            if (id != requestDto.Id)
            {
                return BadRequest(new { message = "ID in URL does not match ID in request body." });
            }

            // Check if group exists
            var existingGroup = await _groupService.GetByIdAsync(id);
            if (existingGroup == null)
            {
                return NotFound(new { message = $"Group with ID '{id}' not found." });
            }

            // Map DTO to domain model
            var group = _mapper.FromRequestDto(requestDto);
            
            // Update group through service (includes validation and business logic)
            await _groupService.UpdateAsync(group);
            
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            // Validation errors from service layer
            return BadRequest(new { message = "Group validation failed.", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while updating the group.", error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a group from the TST system.
    /// </summary>
    /// <param name="id">The unique identifier of the group to delete</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Group successfully deleted</response>
    /// <response code="404">Group with the specified ID was not found</response>
    /// <response code="500">Internal server error occurred</response>
    /// <remarks>
    /// Warning: This operation permanently removes the group from the system.
    /// Consider the impact on user-group relationships before deletion.
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteGroup(string id)
    {
        try
        {
            // Check if group exists
            var existingGroup = await _groupService.GetByIdAsync(id);
            if (existingGroup == null)
            {
                return NotFound(new { message = $"Group with ID '{id}' not found." });
            }

            // Delete group through service
            await _groupService.DeleteAsync(id);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while deleting the group.", error = ex.Message });
        }
    }
}

