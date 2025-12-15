using GrpcServer.Infrastructure.Models.Generic;

namespace GrpcServer.Infrastructure.Models.INM;

public class InmGroup : IBaseGroup
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    // Indicates this entity is stored in-memory and includes the host that created it
    public string InmHost { get; set; } = Environment.MachineName;
}
