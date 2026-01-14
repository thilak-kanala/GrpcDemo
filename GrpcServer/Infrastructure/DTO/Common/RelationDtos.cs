namespace GrpcServer.Infrastructure.DTO.Common;

public static class RelationDtos
{
    public record AddUserToGroupsRequestDto(List<string> GroupIds);
    public record AddUsersToGroupRequestDto(List<string> UserIds);
}
