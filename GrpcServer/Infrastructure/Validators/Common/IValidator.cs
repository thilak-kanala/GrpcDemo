using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Validators.Common;

public interface IValidator<T>
{
    bool IsValid(T entity);
}
