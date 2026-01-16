using Grpc.Core;
using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Protos.Common;
using GrpcServer.Protos.TST;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServer.Infrastructure.GrpcServices.TST;

/// <summary>
/// gRPC service implementation for TST User-Group relationship operations.
/// Provides remote procedure calls for managing many-to-many relationships mirroring the REST controller interface.
/// </summary>
public class TstUserGroupRelationGrpcService : TstUserGroupRelationService.TstUserGroupRelationServiceBase
{
    private readonly IUserGroupRelationService<TstUser, TstGroup> _relationService;
    private readonly TstProtoMapper _mapper;

    public TstUserGroupRelationGrpcService(
        [FromKeyedServices(AppCode.TST)] IUserGroupRelationService<TstUser, TstGroup> relationService,
        [FromKeyedServices(AppCode.TST)] TstProtoMapper mapper)
    {
        _relationService = relationService;
        _mapper = mapper;
    }

    public override async Task<GetUserGroupsResponse> GetUserGroups(
        GetUserGroupsRequest request, 
        ServerCallContext context)
    {
        try
        {
            var groups = await _relationService.GetUserGroupsAsync(request.UserId);
            var response = new GetUserGroupsResponse();
            response.Groups.AddRange(groups.Select(_mapper.ToMessage));
            return response;
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while retrieving user groups: {ex.Message}"));
        }
    }

    public override async Task<AddUserToGroupsResponse> AddUserToGroups(
        AddUserToGroupsRequest request, 
        ServerCallContext context)
    {
        try
        {
            if (request.GroupIds.Count == 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, 
                    "GroupIds list cannot be empty."));
            }

            await _relationService.AddUserToGroupsAsync(request.UserId, request.GroupIds.ToList());
            
            return new AddUserToGroupsResponse
            {
                Success = true,
                Message = $"User '{request.UserId}' successfully added to {request.GroupIds.Count} group(s)."
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while adding user to groups: {ex.Message}"));
        }
    }

    public override async Task<RemoveUserFromGroupResponse> RemoveUserFromGroup(
        RemoveUserFromGroupRequest request, 
        ServerCallContext context)
    {
        try
        {
            await _relationService.RemoveUserFromGroupAsync(request.UserId, request.GroupId);
            
            return new RemoveUserFromGroupResponse
            {
                Success = true,
                Message = $"User '{request.UserId}' successfully removed from group '{request.GroupId}'."
            };
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while removing user from group: {ex.Message}"));
        }
    }

    public override async Task<GetGroupUsersResponse> GetGroupUsers(
        GetGroupUsersRequest request, 
        ServerCallContext context)
    {
        try
        {
            var users = await _relationService.GetGroupUsersAsync(request.GroupId);
            var response = new GetGroupUsersResponse();
            response.Users.AddRange(users.Select(_mapper.ToMessage));
            return response;
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while retrieving group users: {ex.Message}"));
        }
    }

    public override async Task<AddUsersToGroupResponse> AddUsersToGroup(
        AddUsersToGroupRequest request, 
        ServerCallContext context)
    {
        try
        {
            if (request.UserIds.Count == 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, 
                    "UserIds list cannot be empty."));
            }

            await _relationService.AddUsersToGroupAsync(request.GroupId, request.UserIds.ToList());
            
            return new AddUsersToGroupResponse
            {
                Success = true,
                Message = $"{request.UserIds.Count} user(s) successfully added to group '{request.GroupId}'."
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while adding users to group: {ex.Message}"));
        }
    }
}

