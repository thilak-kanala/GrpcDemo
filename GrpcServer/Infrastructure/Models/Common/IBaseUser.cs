namespace GrpcServer.Infrastructure.Models.Common;

public interface IBaseUser
{
    string Id { get; set; }
    string UserName { get; set; }
    string Email { get; set; }
}