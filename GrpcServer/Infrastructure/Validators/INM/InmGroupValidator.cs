using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Validators.Common;

namespace GrpcServer.Infrastructure.Validators.INM;

public class InmGroupValidator : IGroupValidator
{
    public bool IsValid(IBaseGroup entity)
    {
        if (entity is not InmGroup inmGroup)
            return false;

        if (inmGroup.Id < 0)
            return false;

        if (string.IsNullOrWhiteSpace(inmGroup.DisplayName))
            return false;

        if (string.IsNullOrWhiteSpace(inmGroup.InmHost))
            return false;

        return true;
    }
}

