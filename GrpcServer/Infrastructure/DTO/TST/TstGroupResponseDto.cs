using GrpcServer.Infrastructure.DTO.Common;

namespace GrpcServer.Infrastructure.DTO.TST;

public record TstGroupResponseDto(BaseGroupDto BaseGroup, string TstGroupExtension1, string TstGroupExtension2);
