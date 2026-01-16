# gRPC Service Layer Documentation

## Overview
This gRPC service layer mirrors the TST REST controller interface, providing remote procedure call access to user and group management operations.

## Architecture

### Proto Mappers
**IProtoMapper<TEntity, TMessage, TCreateRequest, TUpdateRequest>** - Generic interface for bidirectional mapping between domain entities and proto messages.

**TstProtoMapper** - TST-specific implementation supporting:
- `TstUser` ↔ `TstUserMessage`, `CreateUserRequest`, `UpdateUserRequest`
- `TstGroup` ↔ `TstGroupMessage`, `CreateGroupRequest`, `UpdateGroupRequest`

### gRPC Services
Three services replicate the REST controller functionality:

**TstUserGrpcService** - User CRUD operations
- `GetAllUsers()` - Retrieve all users
- `GetUserById(id)` - Retrieve specific user
- `CreateUser(request)` - Create new user
- `UpdateUser(request)` - Update existing user
- `DeleteUser(id)` - Delete user

**TstGroupGrpcService** - Group CRUD operations
- `GetAllGroups()` - Retrieve all groups
- `GetGroupById(id)` - Retrieve specific group
- `CreateGroup(request)` - Create new group
- `UpdateGroup(request)` - Update existing group
- `DeleteGroup(id)` - Delete group

**TstUserGroupRelationGrpcService** - User-Group relationship operations
- `GetUserGroups(userId)` - Get groups for user
- `AddUserToGroups(userId, groupIds)` - Add user to multiple groups
- `RemoveUserFromGroup(userId, groupId)` - Remove user from group
- `GetGroupUsers(groupId)` - Get users in group
- `AddUsersToGroup(groupId, userIds)` - Add multiple users to group

## Error Handling
All services use gRPC status codes:
- `OK` - Successful operation
- `NOT_FOUND` - Entity not found
- `INVALID_ARGUMENT` - Validation failure
- `INTERNAL` - Server error

## Service Registration
Add to Program.cs:
```csharp
// Register TstProtoMapper
builder.Services.AddKeyedSingleton<TstProtoMapper, TstProtoMapper>(AppCode.TST);

// Map gRPC services
app.MapGrpcService<TstUserGrpcService>();
app.MapGrpcService<TstGroupGrpcService>();
app.MapGrpcService<TstUserGroupRelationGrpcService>();
```

## Design Pattern
The implementation uses:
- **Type-safe generic mappers** - Similar to existing IMapper pattern but for proto messages
- **Service delegation** - gRPC services delegate to existing business logic layer
- **Keyed DI** - Uses AppCode.TST for multi-tenancy support
- **Error translation** - Converts domain exceptions to gRPC status codes

