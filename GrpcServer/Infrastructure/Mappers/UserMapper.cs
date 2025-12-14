using GrpcServer.Infrastructure.DTOs;
using GrpcServer.Infrastructure.Models;

namespace GrpcServer.Infrastructure.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(IUser user)
    {
        return new UserDto(user.Id, user.UserName, user.Email);
    }

    public static User ToEntity(UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            UserName = dto.UserName,
            Email = dto.Email
        };
    }
    
    public static void ApplyPatch(IUser user, UserDto dto)
    {
        if (dto.UserName is not null)
            user.UserName = dto.UserName;
        
        if (dto.Email is not null)
            user.Email = dto.Email;
    }

    // public static MckUser ToEntity(UpdateUserDto dto, int id)
    // {
    //     return new MckUser
    //     {
    //         Id = id,
    //         UserName = dto.UserName,
    //         Email = dto.Email
    //     };
    // }
    //
}

