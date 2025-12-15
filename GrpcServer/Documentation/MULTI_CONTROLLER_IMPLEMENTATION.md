# Multi-Controller Implementation by AppCode

This document describes the multiple controller implementation based on the AppCode enum values.

## Overview

The API now supports multiple backend implementations, each accessible via different path prefixes based on the `AppCode` enum:
- **INM** (InMemory): `/api/v1/inm/...`
- **ABC**: `/api/v1/abc/...`

Each AppCode has its own set of controllers, services, repositories, and data stores.

## Implemented Controllers

### InMemory (INM) Controllers
Located in: `Infrastructure/Controllers/InMemory/`

1. **InmUsersController**
   - Route: `/api/v1/inm/users`
   - Endpoints:
     - `GET /api/v1/inm/users` - List all InMemory users
     - `GET /api/v1/inm/users/{userId}` - Get specific user
     - `POST /api/v1/inm/users` - Create new user
     - `PUT /api/v1/inm/users/{userId}` - Replace user
     - `PATCH /api/v1/inm/users/{userId}` - Partially update user
     - `DELETE /api/v1/inm/users/{userId}` - Delete user

2. **InmGroupsController**
   - Route: `/api/v1/inm/groups`
   - Endpoints:
     - `GET /api/v1/inm/groups` - List all InMemory groups
     - `GET /api/v1/inm/groups/{groupId}` - Get specific group
     - `POST /api/v1/inm/groups` - Create new group
     - `PUT /api/v1/inm/groups/{groupId}` - Replace group
     - `PATCH /api/v1/inm/groups/{groupId}` - Partially update group
     - `DELETE /api/v1/inm/groups/{groupId}` - Delete group

3. **InmUserGroupRelationsController**
   - Route: `/api/v1/inm/user-group-relations`
   - Endpoints:
     - `GET /api/v1/inm/user-group-relations/users/{userId}/groups` - Get user's groups
     - `POST /api/v1/inm/user-group-relations/users/{userId}/groups` - Add user to groups
     - `DELETE /api/v1/inm/user-group-relations/users/{userId}/groups/{groupId}` - Remove user from group
     - `GET /api/v1/inm/user-group-relations/groups/{groupId}/users` - Get group's users
     - `POST /api/v1/inm/user-group-relations/groups/{groupId}/users` - Add users to group
     - `DELETE /api/v1/inm/user-group-relations/groups/{groupId}/users/{userId}` - Remove user from group

### ABC Controllers
Located in: `Infrastructure/Controllers/ABC/`

1. **AbcUsersController**
   - Route: `/api/v1/abc/users`
   - Endpoints: Same structure as InmUsersController

2. **AbcGroupsController**
   - Route: `/api/v1/abc/groups`
   - Endpoints: Same structure as InmGroupsController

3. **AbcUserGroupRelationsController**
   - Route: `/api/v1/abc/user-group-relations`
   - Endpoints: Same structure as InmUserGroupRelationsController

## Architecture

### Dependency Injection with Keyed Services

All services and repositories are registered using .NET's keyed service provider feature:

```csharp
// InMemory registrations
builder.Services.AddKeyedSingleton<IUserRepository, InMemoryUserRepository>(AppCode.INM);
builder.Services.AddKeyedScoped<IUserService, InMemoryUserService>(AppCode.INM);

// ABC registrations
builder.Services.AddKeyedSingleton<IUserRepository, AbcUserRepository>(AppCode.ABC);
builder.Services.AddKeyedScoped<IUserService, AbcUserService>(AppCode.ABC);
```

Controllers inject services using the `[FromKeyedServices]` attribute:

```csharp
public InmUsersController([FromKeyedServices(AppCode.INM)] IUserService userService)
{
    _userService = userService;
}
```

### Mapper Classes

All mappers are now static classes for simplicity:
- `InMemoryUserMapper` - Maps between `InMemoryUser` and DTOs
- `InMemoryGroupMapper` - Maps between `InMemoryGroup` and DTOs
- `AbcUserMapper` - Maps between `AbcUser` and DTOs
- `AbcGroupMapper` - Maps between `AbcGroup` and DTOs

### Data Models

Each AppCode implementation has its own entity types:

**InMemory:**
- `InMemoryUser` (Id, UserName, Email, InMemoryHost)
- `InMemoryGroup` (Id, DisplayName, InMemoryHost)

**ABC:**
- `AbcUser` (Id, UserName, Email, SourceSystem)
- `AbcGroup` (Id, DisplayName, TenantId)

## Testing the API

### Test InMemory Users
```bash
# Get all InMemory users
curl -X GET http://localhost:5000/api/v1/inm/users

# Create InMemory user
curl -X POST http://localhost:5000/api/v1/inm/users \
  -H "Content-Type: application/json" \
  -d '{"userName":"test.user","email":"test@example.com","inMemoryHost":"local"}'
```

### Test ABC Users
```bash
# Get all ABC users
curl -X GET http://localhost:5000/api/v1/abc/users

# Create ABC user
curl -X POST http://localhost:5000/api/v1/abc/users \
  -H "Content-Type: application/json" \
  -d '{"userName":"abc.user","email":"abc@example.com","sourceSystem":"ABC_System"}'
```

### Test Relations
```bash
# Get InMemory user's groups
curl -X GET http://localhost:5000/api/v1/inm/user-group-relations/users/1/groups

# Add ABC user to groups
curl -X POST http://localhost:5000/api/v1/abc/user-group-relations/users/1/groups \
  -H "Content-Type: application/json" \
  -d '{"groupIds":[1,2,3]}'
```

## Adding New AppCode Implementations

To add a new AppCode implementation (e.g., MCK, TST, DEM):

1. Create entity models in `Infrastructure/Models/{AppCode}/`
2. Create DTOs in `Infrastructure/Models/{AppCode}/DTO/`
3. Create static mapper class in `Infrastructure/Mappers/{AppCode}/`
4. Create repositories in `Infrastructure/Repositories/{AppCode}/`
5. Create services in `Infrastructure/Services/{AppCode}/`
6. Create controllers in `Infrastructure/Controllers/{AppCode}/`
7. Register in `Program.cs` with keyed DI using the AppCode enum value
8. Update this documentation

## Benefits of This Approach

1. **Isolation**: Each AppCode has completely isolated data stores
2. **Flexibility**: Different implementations can have different business logic
3. **Scalability**: Easy to add new AppCode implementations
4. **Type Safety**: Strong typing with specific entity types per AppCode
5. **Clean URLs**: Clear API structure with AppCode in the path
6. **Swagger Support**: All endpoints documented and testable via Swagger UI

