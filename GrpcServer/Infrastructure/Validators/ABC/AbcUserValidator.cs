using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.Common;
using GrpcServer.Infrastructure.Validators.Common;

namespace GrpcServer.Infrastructure.Validators.ABC;

public class AbcUserValidator : IUserValidator
{
    public bool IsValid(IBaseUser entity)
    {
        if (entity is not AbcUser abcUser)
            return false;

        if (abcUser.Id < 0)
            return false;

        if (string.IsNullOrWhiteSpace(abcUser.UserName))
            return false;

        if (string.IsNullOrWhiteSpace(abcUser.Email) || !abcUser.Email.Contains('@'))
            return false;

        if (string.IsNullOrWhiteSpace(abcUser.SourceSystem))
            return false;

        return true;
    }
}

