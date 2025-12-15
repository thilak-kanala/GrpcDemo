using GrpcServer.Infrastructure.Models.Generic;

namespace GrpcServer.Infrastructure.Models.ABC;

public class AbcGroup : IBaseGroup
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    // Extended property indicating the tenant that owns this group
    public string TenantId { get; set; } = "default-tenant";
}

