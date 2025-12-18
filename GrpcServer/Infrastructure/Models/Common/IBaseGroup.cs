namespace GrpcServer.Infrastructure.Models.Common;

public interface IBaseGroup
{
    int Id { get; set; }
    string DisplayName { get; set; }
}