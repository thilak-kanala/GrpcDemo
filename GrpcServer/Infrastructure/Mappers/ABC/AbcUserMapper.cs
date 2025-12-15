using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.ABC.DTO;

namespace GrpcServer.Infrastructure.Mappers.ABC;

public static class AbcUserMapper
{
    public static AbcUserResponseDto ToResponseDto(AbcUser entity)
    {
        return new AbcUserResponseDto(
            entity.Id,
            entity.UserName,
            entity.Email,
            entity.SourceSystem
        );
    }

    public static AbcUser FromRequestDto(AbcUserRequestDto dto)
    {
        return new AbcUser
        {
            Id = dto.Id,
            UserName = dto.UserName ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            SourceSystem = dto.SourceSystem ?? "ExternalSystem"
        };
    }

    public static void ApplyPatch(AbcUser entity, AbcUserRequestDto dto)
    {
        if (dto.UserName is not null)
            entity.UserName = dto.UserName;
        
        if (dto.Email is not null)
            entity.Email = dto.Email;
        
        if (dto.SourceSystem is not null)
            entity.SourceSystem = dto.SourceSystem;
    }
}
