using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Models.INM.DTO;

namespace GrpcServer.Infrastructure.Mappers.INM;

public static class InmUserMapper
{
    public static InmUserResponseDto ToResponseDto(InmUser entity)
    {
        return new InmUserResponseDto(
            entity.Id,
            entity.UserName,
            entity.Email,
            entity.InmHost
        );
    }

    public static InmUser FromRequestDto(InmUserRequestDto dto)
    {
        return new InmUser
        {
            Id = dto.Id,
            UserName = dto.UserName ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            InmHost = dto.InMemoryHost ?? System.Environment.MachineName
        };
    }

    public static void ApplyPatch(InmUser entity, InmUserRequestDto dto)
    {
        if (dto.UserName is not null)
            entity.UserName = dto.UserName;
        
        if (dto.Email is not null)
            entity.Email = dto.Email;
        
        if (dto.InMemoryHost is not null)
            entity.InmHost = dto.InMemoryHost;
    }
}
