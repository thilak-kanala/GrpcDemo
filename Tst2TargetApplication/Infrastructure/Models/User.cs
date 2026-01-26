namespace Tst2TargetApplication.Infrastructure.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IsActive { get; set; } = string.Empty;
    public string[] Devices { get; set; } = [];
    public string PreferredLanguage { get; set; } = string.Empty;
    public int[] GroupIds { get; set;  } = [];
}

/*
   "Id": 1,
   "Username": "alice",
   "Email": "alice@example.com",
   "IsActive": "false",
   "Devices": ["iPhone 13", "MacBook Pro"],
   "PreferredLanguage": "en",
   "GroupIds": [1, 2]
 */
