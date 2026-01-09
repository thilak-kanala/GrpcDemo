using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Validators.TST;

public class TstGroupValidator : IValidator<TstGroup>
{
    // Custom demonstration validation logic:
    // - Id must not be empty
    // - DisplayName must not be empty
    // - TstGroupExtension1 must be at least 5 characters long (demo rule)
    public bool IsValid(TstGroup tstGroup)
    {
        if (string.IsNullOrWhiteSpace(tstGroup.Id))
            return false;

        if (string.IsNullOrWhiteSpace(tstGroup.DisplayName))
            return false;

        // DEMO: Extension1 must be at least 5 chars
        if (string.IsNullOrWhiteSpace(tstGroup.TstGroupExtension1) || tstGroup.TstGroupExtension1.Length < 5)
            return false;

        return true;
    }
}
