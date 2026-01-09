namespace TstTargetApplication.Infrastructure.Models;

public class UserWithGroupIds
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = string.Empty;
    public List<int> GroupIds { get; set; } = new();
}

