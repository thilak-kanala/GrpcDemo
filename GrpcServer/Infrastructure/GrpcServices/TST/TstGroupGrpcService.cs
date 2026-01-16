using Grpc.Core;
using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Protos.Common;
using GrpcServer.Protos.TST;

namespace GrpcServer.Infrastructure.GrpcServices.TST;

/// <summary>
/// gRPC service implementation for TST Group operations.
/// Provides remote procedure calls for group management mirroring the REST controller interface.
/// </summary>
public class TstGroupGrpcService : TstGroupGrpcServiceDefinition.TstGroupGrpcServiceDefinitionBase
{
    private readonly IGroupService<TstGroup> _groupService;
    private readonly TstProtoMapper _mapper;

    public TstGroupGrpcService(
        [FromKeyedServices(AppCode.TST)] IGroupService<TstGroup> groupService,
        [FromKeyedServices(AppCode.TST)] TstProtoMapper mapper)
    {
        _groupService = groupService;
        _mapper = mapper;
    }

    public override async Task<GetAllGroupsResponse> GetAllGroups(
        GetAllGroupsRequest request, 
        ServerCallContext context)
    {
        try
        {
            var groups = await _groupService.GetAllAsync();
            var response = new GetAllGroupsResponse();
            response.Groups.AddRange(groups.Select(_mapper.ToMessage));
            return response;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while retrieving groups: {ex.Message}"));
        }
    }

    public override async Task<GetGroupByIdResponse> GetGroupById(
        GetGroupByIdRequest request, 
        ServerCallContext context)
    {
        try
        {
            var group = await _groupService.GetByIdAsync(request.Id);
            
            if (group == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, 
                    $"Group with ID '{request.Id}' not found."));
            }

            return new GetGroupByIdResponse
            {
                Group = _mapper.ToMessage(group)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while retrieving the group: {ex.Message}"));
        }
    }

    public override async Task<TstGroupResponse> CreateGroup(
        TstGroupRequest request, 
        ServerCallContext context)
    {
        try
        {
            var group = _mapper.FromRequest(request);
            await _groupService.AddAsync(group);
            
            return new TstGroupResponse
            {
                Group = _mapper.ToMessage(group)
            };
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, 
                $"Group validation failed: {ex.Message}"));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while creating the group: {ex.Message}"));
        }
    }

    public override async Task<TstGroupResponse> UpdateGroup(
        TstGroupRequest request, 
        ServerCallContext context)
    {
        try
        {
            var existingGroup = await _groupService.GetByIdAsync(request.Base.Id);
            if (existingGroup == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, 
                    $"Group with ID '{request.Base.Id}' not found."));
            }

            var group = _mapper.FromRequest(request);
            await _groupService.UpdateAsync(group);
            
            return new TstGroupResponse
            {
                Group = _mapper.ToMessage(group)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, 
                $"Group validation failed: {ex.Message}"));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while updating the group: {ex.Message}"));
        }
    }

    public override async Task<DeleteGroupResponse> DeleteGroup(
        DeleteGroupRequest request, 
        ServerCallContext context)
    {
        try
        {
            var existingGroup = await _groupService.GetByIdAsync(request.Id);
            if (existingGroup == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, 
                    $"Group with ID '{request.Id}' not found."));
            }

            await _groupService.DeleteAsync(request.Id);
            
            return new DeleteGroupResponse
            {
                Success = true,
                Message = $"Group with ID '{request.Id}' successfully deleted."
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while deleting the group: {ex.Message}"));
        }
    }
}

