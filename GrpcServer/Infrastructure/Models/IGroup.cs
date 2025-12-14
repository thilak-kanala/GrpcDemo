namespace GrpcServer.Infrastructure.Models;

public interface IGroup
{
    int Id { get; set; }
    string DisplayName { get; set; }
}