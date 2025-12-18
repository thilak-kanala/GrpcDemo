using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Validators.Common;

namespace GrpcServer.Infrastructure.Validators.ABC;

public class AbcGroupValidator : IGroupValidator
{
    public bool IsValid(IBaseGroup entity)
    {
        if (entity is not AbcGroup abcGroup)
            return false;

        if (abcGroup.Id < 0)
            return false;

        if (string.IsNullOrWhiteSpace(abcGroup.DisplayName))
            return false;

        if (string.IsNullOrWhiteSpace(abcGroup.TenantId))
            return false;

        return true;
    }
}

