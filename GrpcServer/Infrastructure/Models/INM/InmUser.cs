using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Models.INM;

public class InmUser : IBaseUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Indicates this entity is stored in-memory and includes the host that created it
    public string InmHost { get; set; } = Environment.MachineName;
}
