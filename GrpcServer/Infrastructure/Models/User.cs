namespace GrpcServer.Infrastructure.Models;

public class User : IUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}