using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Models.INM.DTO;

namespace GrpcServer.Infrastructure.Mappers.INM;

public sealed class InmMapper : 
    IMapper<InmUser, InmUserRequestDto, InmUserResponseDto>,
    IMapper<InmGroup, InmGroupRequestDto, InmGroupResponseDto>
{
    // User mapping methods
    public InmUserResponseDto ToResponseDto(InmUser entity) =>
        new(entity.Id, entity.UserName, entity.Email, entity.InmHost);

    public InmUser FromRequestDto(InmUserRequestDto dto) =>
        new()
        {
            Id = dto.Id,
            UserName = dto.UserName ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            InmHost = dto.InMemoryHost ?? Environment.MachineName
        };

    public void ApplyPatch(InmUser entity, InmUserRequestDto dto)
    {
        entity.UserName = dto.UserName ?? entity.UserName;
        entity.Email = dto.Email ?? entity.Email;
        entity.InmHost = dto.InMemoryHost ?? entity.InmHost;
    }

    // Group mapping methods
    public InmGroupResponseDto ToResponseDto(InmGroup entity) =>
        new(entity.Id, entity.DisplayName, entity.InmHost);

    public InmGroup FromRequestDto(InmGroupRequestDto dto) =>
        new()
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName ?? string.Empty,
            InmHost = dto.InMemoryHost ?? Environment.MachineName
        };

    public void ApplyPatch(InmGroup entity, InmGroupRequestDto dto)
    {
        entity.DisplayName = dto.DisplayName ?? entity.DisplayName;
        entity.InmHost = dto.InMemoryHost ?? entity.InmHost;
    }
}

