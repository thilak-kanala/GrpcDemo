namespace GrpcServer.Infrastructure.DTOs;

public record AddUserToGroupsDto(List<int> GroupIds);

public record AddUsersToGroupDto(List<int> UserIds);

