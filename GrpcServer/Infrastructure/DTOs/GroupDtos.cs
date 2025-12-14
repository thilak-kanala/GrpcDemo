namespace GrpcServer.Infrastructure.DTOs;

// public record CreateGroupDto(string DisplayName);
// public record UpdateGroupDto(string DisplayName);
// public record PatchGroupDto(string? DisplayName);

public record GroupDto(int Id, string DisplayName);

