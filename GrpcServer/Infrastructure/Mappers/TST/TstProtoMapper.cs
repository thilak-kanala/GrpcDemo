using GrpcServer.Infrastructure.Mappers.Common;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Protos.Common;
using GrpcServer.Protos.TST;

namespace GrpcServer.Infrastructure.Mappers.TST;

/// <summary>
/// TST-specific implementation of IProtoMapper for bidirectional mapping between domain entities and proto messages.
/// Implements type-safe conversions for TstUser and TstGroup entities.
/// </summary>
public sealed class TstProtoMapper :
    IProtoMapper<TstUser, TstUserMessage, TstUserRequest>,
    IProtoMapper<TstGroup, TstGroupMessage, TstGroupRequest>
{
    // User mappings
    public TstUserMessage ToMessage(TstUser entity)
    {
        return new TstUserMessage
        {
            Base = new BaseUserMessage
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email
            },
            TstUserExtension1 = entity.TstUserExtension1,
            TstUserExtension2 = entity.TstUserExtension2
        };
    }

    public TstUser FromRequest(TstUserRequest request)
    {
        return new TstUser
        {
            Id = request.Base.Id,
            UserName = request.Base.UserName,
            Email = request.Base.Email,
            TstUserExtension1 = request.TstUserExtension1,
            TstUserExtension2 = request.TstUserExtension2
        };
    }

    // Group mappings
    public TstGroupMessage ToMessage(TstGroup entity)
    {
        return new TstGroupMessage
        {
            Base = new BaseGroupMessage
            {
                Id = entity.Id,
                DisplayName = entity.DisplayName
            },
            TstGroupExtension1 = entity.TstGroupExtension1,
            TstGroupExtension2 = entity.TstGroupExtension2
        };
    }

    public TstGroup FromRequest(TstGroupRequest request)
    {
        return new TstGroup
        {
            Id = request.Base.Id,
            DisplayName = request.Base.DisplayName,
            TstGroupExtension1 = request.TstGroupExtension1,
            TstGroupExtension2 = request.TstGroupExtension2
        };
    }
}

