namespace GrpcServer.Infrastructure.Models;

public interface IUser
{
    int Id { get; set; }
    string UserName { get; set; }
    string Email { get; set; }
}