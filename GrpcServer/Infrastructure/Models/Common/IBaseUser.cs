namespace GrpcServer.Infrastructure.Models.Common;

public interface IBaseUser
{
    int Id { get; set; }
    string UserName { get; set; }
    string Email { get; set; }
}