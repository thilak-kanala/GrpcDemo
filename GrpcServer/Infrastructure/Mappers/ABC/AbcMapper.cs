using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.ABC.DTO;

namespace GrpcServer.Infrastructure.Mappers.ABC;

public sealed class AbcMapper : 
    IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto>,
    IMapper<AbcGroup, AbcGroupRequestDto, AbcGroupResponseDto>
{
    // User mapping methods
    public AbcUserResponseDto ToResponseDto(AbcUser entity) =>
        new(entity.Id, entity.UserName, entity.Email, entity.SourceSystem);

    public AbcUser FromRequestDto(AbcUserRequestDto dto) =>
        new()
        {
            Id = dto.Id,
            UserName = dto.UserName ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            SourceSystem = dto.SourceSystem ?? "ExternalSystem"
        };

    public void ApplyPatch(AbcUser entity, AbcUserRequestDto dto)
    {
        entity.UserName = dto.UserName ?? entity.UserName;
        entity.Email = dto.Email ?? entity.Email;
        entity.SourceSystem = dto.SourceSystem ?? entity.SourceSystem;
    }

    // Group mapping methods
    public AbcGroupResponseDto ToResponseDto(AbcGroup entity) =>
        new(entity.Id, entity.DisplayName, entity.TenantId);

    public AbcGroup FromRequestDto(AbcGroupRequestDto dto) =>
        new()
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName ?? string.Empty,
            TenantId = dto.TenantId ?? "default-tenant"
        };

    public void ApplyPatch(AbcGroup entity, AbcGroupRequestDto dto)
    {
        entity.DisplayName = dto.DisplayName ?? entity.DisplayName;
        entity.TenantId = dto.TenantId ?? entity.TenantId;
    }
}

