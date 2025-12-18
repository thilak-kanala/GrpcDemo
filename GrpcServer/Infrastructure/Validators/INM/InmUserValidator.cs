using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Models.INM;
using GrpcServer.Infrastructure.Validators.Common;

namespace GrpcServer.Infrastructure.Validators.INM;

public class InmUserValidator : IUserValidator
{
    public bool IsValid(IBaseUser entity)
    {
        if (entity is not InmUser inmUser)
            return false;

        if (inmUser.Id < 0)
            return false;

        if (string.IsNullOrWhiteSpace(inmUser.UserName))
            return false;

        if (string.IsNullOrWhiteSpace(inmUser.Email) || !inmUser.Email.Contains('@'))
            return false;

        if (string.IsNullOrWhiteSpace(inmUser.InmHost))
            return false;

        return true;
    }
}

