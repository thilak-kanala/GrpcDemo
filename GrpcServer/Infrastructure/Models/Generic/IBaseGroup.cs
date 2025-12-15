namespace GrpcServer.Infrastructure.Models.Generic;

public interface IBaseGroup
{
    int Id { get; set; }
    string DisplayName { get; set; }
}