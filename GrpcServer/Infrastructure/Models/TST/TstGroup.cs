using GrpcServer.Infrastructure.Models.Common;

namespace GrpcServer.Infrastructure.Models.TST;

public class TstGroup : IBaseGroup
{
    public required string Id { get; set; } = string.Empty;
    public required string DisplayName { get; set; } = string.Empty;
    public required string TstGroupExtension1 { get; set; } = string.Empty;
    public string TstGroupExtension2 { get; set; } = string.Empty;
}
