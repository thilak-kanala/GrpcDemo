using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;
using GrpcServer.Tests.Infrastructure.Models.TST.DTO;

namespace GrpcServer.Tests.Infrastructure.Mappers.TST
{
    public sealed class TstMapper : 
        IMapper<TstUser, TstUserRequestDto, TstUserResponseDto>,
        IMapper<TstGroup, TstGroupRequestDto, TstGroupResponseDto>
    {
        public TstUserResponseDto ToResponseDto(TstUser entity)
        {
            return new TstUserResponseDto(
                entity.Id,
                entity.UserName,
                entity.Email,
                entity.TstUserExtension1,
                entity.TstUserExtension2
            );
        }

        public TstUser FromRequestDto(TstUserRequestDto dto)
        {
            return new TstUser
            {
                Id = dto.Id,
                UserName = dto.UserName,
                Email = dto.Email,
                TstUserExtension1 = dto.TstUserExtension1,
                TstUserExtension2 = dto.TstUserExtension2
            };
        }

        public void ApplyPatch(TstUser entity, TstUserRequestDto dto)
        {
            entity.Id = dto.Id;
            entity.UserName = dto.UserName;
            entity.Email = dto.Email;
            entity.TstUserExtension1 = dto.TstUserExtension1;
            entity.TstUserExtension2 = dto.TstUserExtension2;
        }

        public TstGroupResponseDto ToResponseDto(TstGroup entity)
        {
            return new TstGroupResponseDto(
                entity.Id,
                entity.DisplayName,
                entity.TstGroupExtension1,
                entity.TstGroupExtension2
            );
        }

        public TstGroup FromRequestDto(TstGroupRequestDto dto)
        {
            return new TstGroup
            {
                Id = dto.Id,
                DisplayName = dto.DisplayName,
                TstGroupExtension1 = dto.TstGroupExtension1,
                TstGroupExtension2 = dto.TstGroupExtension2
            };
        }

        public void ApplyPatch(TstGroup entity, TstGroupRequestDto dto)
        {
            entity.Id = dto.Id;
            entity.DisplayName = dto.DisplayName;
            entity.TstGroupExtension1 = dto.TstGroupExtension1;
            entity.TstGroupExtension2 = dto.TstGroupExtension2;
        }
    }
}
