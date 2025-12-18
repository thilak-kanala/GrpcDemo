# Repository Implementation Guide

## Overview
This document describes the User and Group repositories that support CRUD operations for the TstTargetApplication.

## Repository Registration

Add the following to your `Program.cs` to register the repositories:

```csharp
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IGroupRepository, GroupRepository>();
```

## User Repository

### Interface: `IUserRepository`

#### Methods:

1. **GetAllUsersAsync()** - Retrieves all users
   - Returns: `IEnumerable<User>`
   - Use case: User controller GET all endpoint

2. **GetUserByIdAsync(int id)** - Retrieves a specific user
   - Returns: `User?` (null if not found)
   - Use case: User controller GET by ID endpoint

3. **CreateUserAsync(User user)** - Creates a new user
   - Returns: `User` (with auto-generated ID)
   - Use case: User controller POST endpoint

4. **ReplaceUserAsync(int id, User user)** - Replaces entire user (PUT)
   - Returns: `User?` (null if not found)
   - Use case: User controller PUT endpoint

5. **UpdateUserAsync(int id, User user)** - Partially updates user (PATCH)
   - Returns: `User?` (null if not found)
   - Only updates non-null/non-empty properties
   - Use case: User controller PATCH endpoint

6. **DeleteUserAsync(int id)** - Deletes a user
   - Returns: `bool` (true if deleted, false if not found)
   - Use case: User controller DELETE endpoint

7. **GetUserGroupIdsAsync(int userId)** - Gets group IDs for a user
   - Returns: `IEnumerable<int>`
   - Use case: Retrieving user's group memberships

## Group Repository

### Interface: `IGroupRepository`

#### Methods:

1. **GetAllGroupsAsync()** - Retrieves all groups
   - Returns: `IEnumerable<Group>`
   - Use case: Group controller GET all endpoint

2. **GetGroupByIdAsync(int id)** - Retrieves a specific group
   - Returns: `Group?` (null if not found)
   - Use case: Group controller GET by ID endpoint

3. **CreateGroupAsync(Group group)** - Creates a new group
   - Returns: `Group` (with auto-generated ID)
   - Use case: Group controller POST endpoint

4. **ReplaceGroupAsync(int id, Group group)** - Replaces entire group (PUT)
   - Returns: `Group?` (null if not found)
   - Use case: Group controller PUT endpoint

5. **UpdateGroupAsync(int id, Group group)** - Partially updates group (PATCH)
   - Returns: `Group?` (null if not found)
   - Only updates non-null/non-empty properties
   - Use case: Group controller PATCH endpoint

6. **DeleteGroupAsync(int id)** - Deletes a group
   - Returns: `bool` (true if deleted, false if not found)
   - Use case: Group controller DELETE endpoint

## Data Models

### User
```csharp
public record User(int Id, string Username, string Email, string FirstName, string LastName, string PreferredLanguage);
```

### UserWithGroupIds (Internal DTO)
```csharp
public class UserWithGroupIds
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PreferredLanguage { get; set; }
    public List<int> GroupIds { get; set; }
}
```

### Group
```csharp
public record Group(int Id, string Name, string Description, string Priority);
```

## Features

### Thread Safety
- Both repositories use `SemaphoreSlim` for thread-safe operations
- Safe for concurrent requests

### Data Persistence
- Data is loaded from JSON files on startup
- Changes are persisted back to JSON files immediately
- Files located in `Infrastructure/Util/`

### Auto-Increment IDs
- New entities automatically get the next available ID
- ID starts at 1 if no entities exist

### PATCH vs PUT
- **PUT (Replace)**: Replaces all fields with new values
- **PATCH (Update)**: Only updates fields that are provided (non-null/non-empty)

## Example Controller Usage

```csharp
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        var created = await _userRepository.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> ReplaceUser(int id, User user)
    {
        var updated = await _userRepository.ReplaceUserAsync(id, user);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<User>> UpdateUser(int id, User user)
    {
        var updated = await _userRepository.UpdateUserAsync(id, user);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _userRepository.DeleteUserAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
```

