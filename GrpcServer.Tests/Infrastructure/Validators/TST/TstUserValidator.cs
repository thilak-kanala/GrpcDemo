using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Validators.TST;

public class TstUserValidator : IValidator<TstUser>
{
    // Custom demonstration validation logic:
    // - Id must not be empty
    // - UserName must not be empty
    // - Email must contain '@'
    // - TstUserExtension1 must not be 'forbidden' (demo rule)
    public bool IsValid(TstUser tstUser)
    {
        if (string.IsNullOrWhiteSpace(tstUser.Id))
            return false;

        if (string.IsNullOrWhiteSpace(tstUser.UserName))
            return false;

        if (string.IsNullOrWhiteSpace(tstUser.Email) || !tstUser.Email.Contains('@'))
            return false;

        // DEMO: Extension1 must not be 'forbidden'
        if (tstUser.TstUserExtension1 == "forbidden")
            return false;

        return true;
    }
}
