using GrpcServer.Infrastructure.DTO.Common;

namespace GrpcServer.Infrastructure.DTO.TST;

public record TstUserResponseDto(BaseUserDto BaseUser, string TstUserExtension1, string TstUserExtension2);
