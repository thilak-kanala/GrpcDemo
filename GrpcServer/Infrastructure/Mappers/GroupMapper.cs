using GrpcServer.Infrastructure.DTOs;
using GrpcServer.Infrastructure.Models;
using Group = GrpcServer.Infrastructure.Models.Group;

namespace GrpcServer.Infrastructure.Mappers;

public static class GroupMapper
{
    public static GroupDto ToDto(IGroup group)
    {
        return new GroupDto(group.Id, group.DisplayName);
    }

    public static Group ToEntity(GroupDto dto)
    {
        return new Group
        {
            Id = dto.Id,
            DisplayName = dto.DisplayName
        };
    }
    
    public static void ApplyPatch(IGroup group, GroupDto dto)
    {
        if (dto.DisplayName is not null)
            group.DisplayName = dto.DisplayName;
    }

    // public static MckGroup ToEntity(UpdateGroupDto dto, int id)
    // {
    //     return new MckGroup
    //     {
    //         Id = id,
    //         DisplayName = dto.DisplayName
    //     };
    // }
}

