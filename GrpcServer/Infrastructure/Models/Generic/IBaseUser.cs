namespace GrpcServer.Infrastructure.Models.Generic;

public interface IBaseUser
{
    int Id { get; set; }
    string UserName { get; set; }
    string Email { get; set; }
}