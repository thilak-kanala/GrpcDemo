namespace GrpcServer.Infrastructure.Models.Common;

public static class RelationDtos
{
    public record AddUserToGroupsRequestDto(List<int> GroupIds);
    public record AddUsersToGroupRequestDto(List<int> UserIds);
}
