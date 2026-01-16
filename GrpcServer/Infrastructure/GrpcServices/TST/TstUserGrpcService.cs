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
/// gRPC service implementation for TST User operations.
/// Provides remote procedure calls for user management mirroring the REST controller interface.
/// </summary>
public class TstUserGrpcService : TstUserService.TstUserServiceBase
{
    private readonly IUserService<TstUser> _userService;
    private readonly TstProtoMapper _mapper;

    public TstUserGrpcService(
        [FromKeyedServices(AppCode.TST)] IUserService<TstUser> userService,
        [FromKeyedServices(AppCode.TST)] TstProtoMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public override async Task<GetAllUsersResponse> GetAllUsers(
        GetAllUsersRequest request, 
        ServerCallContext context)
    {
        try
        {
            var users = await _userService.GetAllAsync();
            var response = new GetAllUsersResponse();
            response.Users.AddRange(users.Select(_mapper.ToMessage));
            return response;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while retrieving users: {ex.Message}"));
        }
    }

    public override async Task<GetUserByIdResponse> GetUserById(
        GetUserByIdRequest request, 
        ServerCallContext context)
    {
        try
        {
            var user = await _userService.GetByIdAsync(request.Id);
            
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, 
                    $"User with ID '{request.Id}' not found."));
            }

            return new GetUserByIdResponse
            {
                User = _mapper.ToMessage(user)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while retrieving the user: {ex.Message}"));
        }
    }

    public override async Task<TstUserResponse> CreateUser(
        TstUserRequest request, 
        ServerCallContext context)
    {
        try
        {
            var user = _mapper.FromRequest(request);
            await _userService.AddAsync(user);
            
            return new TstUserResponse
            {
                User = _mapper.ToMessage(user)
            };
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, 
                $"User validation failed: {ex.Message}"));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while creating the user: {ex.Message}"));
        }
    }

    public override async Task<TstUserResponse> UpdateUser(
        TstUserRequest request, 
        ServerCallContext context)
    {
        try
        {
            var existingUser = await _userService.GetByIdAsync(request.Base.Id);
            if (existingUser == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, 
                    $"User with ID '{request.Base.Id}' not found."));
            }

            var user = _mapper.FromRequest(request);
            await _userService.UpdateAsync(user);
            
            return new TstUserResponse
            {
                User = _mapper.ToMessage(user)
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, 
                $"User validation failed: {ex.Message}"));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while updating the user: {ex.Message}"));
        }
    }

    public override async Task<DeleteUserResponse> DeleteUser(
        DeleteUserRequest request, 
        ServerCallContext context)
    {
        try
        {
            var existingUser = await _userService.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, 
                    $"User with ID '{request.Id}' not found."));
            }

            await _userService.DeleteAsync(request.Id);
            
            return new DeleteUserResponse
            {
                Success = true,
                Message = $"User with ID '{request.Id}' successfully deleted."
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, 
                $"An error occurred while deleting the user: {ex.Message}"));
        }
    }
}

