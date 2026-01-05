using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Tests.Infrastructure.Models.TST;

public class TstUser : IBaseUser
{
    public required string Id { get; set; } = string.Empty;
    public required string UserName { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
    public string TstUserExtension1 { get; set; } = string.Empty;
    public string TstUserExtension2 { get; set; } = string.Empty;
}
