using GrpcServer.Infrastructure.DTO.Common;
using GrpcServer.Infrastructure.DTO.TST;
using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.TST;

namespace GrpcServer.Infrastructure.Mappers.TST;

public sealed class TstMapper : 
    IMapper<TstUser, TstUserRequestDto, TstUserResponseDto>,
    IMapper<TstGroup, TstGroupRequestDto, TstGroupResponseDto>
{
    public TstUserResponseDto ToResponseDto(TstUser entity)
    {
        var baseUser = new BaseUserDto(entity.Id, entity.UserName, entity.Email);
        return new TstUserResponseDto(
            baseUser,
            entity.TstUserExtension1,
            entity.TstUserExtension2
        );
    }

    public TstUser FromRequestDto(TstUserRequestDto dto)
    {
        return new TstUser
        {
            Id = dto.BaseUser.Id,
            UserName = dto.BaseUser.UserName,
            Email = dto.BaseUser.Email,
            TstUserExtension1 = dto.TstUserExtension1,
            TstUserExtension2 = dto.TstUserExtension2
        };
    }

    public TstGroupResponseDto ToResponseDto(TstGroup entity)
    {
        var baseGroup = new BaseGroupDto(entity.Id, entity.DisplayName);
        return new TstGroupResponseDto(
            baseGroup,
            entity.TstGroupExtension1,
            entity.TstGroupExtension2
        );
    }

    public TstGroup FromRequestDto(TstGroupRequestDto dto)
    {
        return new TstGroup
        {
            Id = dto.BaseGroup.Id,
            DisplayName = dto.BaseGroup.DisplayName,
            TstGroupExtension1 = dto.TstGroupExtension1,
            TstGroupExtension2 = dto.TstGroupExtension2
        };
    }
}