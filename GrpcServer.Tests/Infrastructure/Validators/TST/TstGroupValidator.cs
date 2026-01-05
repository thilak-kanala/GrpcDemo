using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Validators.TST;

public class TstGroupValidator : IGroupValidator
{
    public bool IsValid(IBaseGroup entity)
    {
        if (entity is not TstGroup inmGroup)
            return false;

        if (string.IsNullOrWhiteSpace(inmGroup.Id))
            return false;

        if (string.IsNullOrWhiteSpace(inmGroup.DisplayName))
            return false;
        
        if (string.IsNullOrWhiteSpace(inmGroup.TstGroupExtension1))
            return false;
        
        if (string.IsNullOrWhiteSpace(inmGroup.TstGroupExtension2))
            return false;

        return true;
    }
}

