using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Models.INM.DTO;

namespace GrpcServer.Infrastructure.Mappers.INM;

public static class InmGroupMapper
{
    public static InmGroupResponseDto ToResponseDto(InmGroup entity)
    {
        return new InmGroupResponseDto(
            entity.Id,
            entity.DisplayName,
            entity.InmHost
        );
    }

    public static InmGroup FromRequestDto(InmGroupRequestDto dto)
    {
        return new InmGroup
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName ?? string.Empty,
            InmHost = dto.InMemoryHost ?? System.Environment.MachineName
        };
    }

    public static void ApplyPatch(InmGroup entity, InmGroupRequestDto dto)
    {
        if (dto.DisplayName is not null)
            entity.DisplayName = dto.DisplayName;
        
        if (dto.InMemoryHost is not null)
            entity.InmHost = dto.InMemoryHost;
    }
}
