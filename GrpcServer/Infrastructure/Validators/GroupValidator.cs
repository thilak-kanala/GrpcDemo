using GrpcServer.Infrastructure.DTOs;

namespace GrpcServer.Infrastructure.Validators;

public static class GroupValidator
{
    public static (bool IsValid, List<string> Errors) Validate(GroupDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.DisplayName))
            errors.Add("DisplayName is required");
        else if (dto.DisplayName.Length < 3)
            errors.Add("DisplayName must be at least 3 characters");

        return (errors.Count == 0, errors);
    }

    // public static (bool IsValid, List<string> Errors) Validate(UpdateGroupDto dto)
    // {
    //     var errors = new List<string>();
    //
    //     if (string.IsNullOrWhiteSpace(dto.DisplayName))
    //         errors.Add("DisplayName is required");
    //     else if (dto.DisplayName.Length < 3)
    //         errors.Add("DisplayName must be at least 3 characters");
    //
    //     return (errors.Count == 0, errors);
    // }
    //
    // public static (bool IsValid, List<string> Errors) Validate(PatchGroupDto dto)
    // {
    //     var errors = new List<string>();
    //
    //     if (dto.DisplayName is not null && dto.DisplayName.Length < 3)
    //         errors.Add("DisplayName must be at least 3 characters");
    //
    //     return (errors.Count == 0, errors);
    // }
}

