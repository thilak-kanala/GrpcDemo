using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Tests.Infrastructure.Models.TST;

namespace GrpcServer.Tests.Infrastructure.Validators.TST;

public class TstUserValidator : IUserValidator
{
    public bool IsValid(IBaseUser entity)
    {
        if (entity is not TstUser inmUser)
            return false;

        if (string.IsNullOrWhiteSpace(inmUser.Id))
            return false;

        if (string.IsNullOrWhiteSpace(inmUser.UserName))
            return false;

        if (string.IsNullOrWhiteSpace(inmUser.Email) || !inmUser.Email.Contains('@'))
            return false;
        
        if (string.IsNullOrWhiteSpace(inmUser.TstUserExtension1))
            return false;
        
        if (string.IsNullOrWhiteSpace(inmUser.TstUserExtension2))
            return false;

        return true;
    }
}

