using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.ABC.DTO;

namespace GrpcServer.Infrastructure.Mappers.ABC;

public static class AbcGroupMapper
{
    public static AbcGroupResponseDto ToResponseDto(AbcGroup entity)
    {
        return new AbcGroupResponseDto(
            entity.Id,
            entity.DisplayName,
            entity.TenantId
        );
    }

    public static AbcGroup FromRequestDto(AbcGroupRequestDto dto)
    {
        return new AbcGroup
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName ?? string.Empty,
            TenantId = dto.TenantId ?? "default-tenant"
        };
    }

    public static void ApplyPatch(AbcGroup entity, AbcGroupRequestDto dto)
    {
        if (dto.DisplayName is not null)
            entity.DisplayName = dto.DisplayName;
        
        if (dto.TenantId is not null)
            entity.TenantId = dto.TenantId;
    }
}
