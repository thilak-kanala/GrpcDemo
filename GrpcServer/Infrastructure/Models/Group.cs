namespace GrpcServer.Infrastructure.Models;

public class Group : IGroup
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}