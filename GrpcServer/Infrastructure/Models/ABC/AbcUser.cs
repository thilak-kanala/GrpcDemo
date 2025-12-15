using GrpcServer.Infrastructure.Models.Generic;

namespace GrpcServer.Infrastructure.Models.ABC;

public class AbcUser : IBaseUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Extended property indicating the external source/system for this user
    public string SourceSystem { get; set; } = "ExternalSystem";
}

