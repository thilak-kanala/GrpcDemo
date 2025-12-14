using GrpcServer.Infrastructure.DTOs;

namespace GrpcServer.Infrastructure.Validators;

public static class UserValidator
{
    public static (bool IsValid, List<string> Errors) Validate(UserDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.UserName))
            errors.Add("UserName is required");
        else if (dto.UserName.Length < 3)
            errors.Add("UserName must be at least 3 characters");

        if (string.IsNullOrWhiteSpace(dto.Email))
            errors.Add("Email is required");
        else if (!IsValidEmail(dto.Email))
            errors.Add("Email is not valid");

        return (errors.Count == 0, errors);
    }

    private static bool IsValidEmail(string email)
    {
        // Simple email validation
        return email.Contains('@') && email.Contains('.');
    }
    
    // public static (bool IsValid, List<string> Errors) Validate(UpdateUserDto dto)
    // {
    //     var errors = new List<string>();
    //
    //     if (string.IsNullOrWhiteSpace(dto.UserName))
    //         errors.Add("UserName is required");
    //     else if (dto.UserName.Length < 3)
    //         errors.Add("UserName must be at least 3 characters");
    //
    //     if (string.IsNullOrWhiteSpace(dto.Email))
    //         errors.Add("Email is required");
    //     else if (!IsValidEmail(dto.Email))
    //         errors.Add("Email is not valid");
    //
    //     return (errors.Count == 0, errors);
    // }
    //
    // public static (bool IsValid, List<string> Errors) Validate(PatchUserDto dto)
    // {
    //     var errors = new List<string>();
    //
    //     if (dto.UserName is not null && dto.UserName.Length < 3)
    //         errors.Add("UserName must be at least 3 characters");
    //
    //     if (dto.Email is not null && !IsValidEmail(dto.Email))
    //         errors.Add("Email is not valid");
    //
    //     return (errors.Count == 0, errors);
    // }
}

