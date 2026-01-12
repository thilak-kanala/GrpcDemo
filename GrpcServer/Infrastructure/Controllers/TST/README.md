# TST Controllers Documentation

## Overview
Three comprehensive REST API controllers have been created for the TST (Test) system, providing complete CRUD operations and relationship management.

## Controllers

### 1. **TstUserController** (`/api/tst/users`)
Manages TST user entities with full CRUD operations.

**Endpoints:**
- `GET /api/tst/users` - Retrieve all users
- `GET /api/tst/users/{id}` - Retrieve user by ID
- `POST /api/tst/users` - Create new user
- `PUT /api/tst/users/{id}` - Update existing user
- `DELETE /api/tst/users/{id}` - Delete user

**Features:**
- Comprehensive validation via TstUserValidator
- Email normalization (lowercase)
- Proper error handling with descriptive messages
- DTOs for request/response separation

### 2. **TstGroupController** (`/api/tst/groups`)
Manages TST group entities with full CRUD operations.

**Endpoints:**
- `GET /api/tst/groups` - Retrieve all groups
- `GET /api/tst/groups/{id}` - Retrieve group by ID
- `POST /api/tst/groups` - Create new group
- `PUT /api/tst/groups/{id}` - Update existing group
- `DELETE /api/tst/groups/{id}` - Delete group

**Features:**
- Validation via TstGroupValidator
- DisplayName whitespace trimming
- Consistent error handling
- DTOs for request/response separation

### 3. **TstUserGroupRelationController** (`/api/tst`)
Manages many-to-many relationships between users and groups.

**Endpoints:**
- `GET /api/tst/users/{userId}/groups` - Get all groups for a user
- `POST /api/tst/users/{userId}/groups` - Add user to multiple groups
- `DELETE /api/tst/users/{userId}/groups/{groupId}` - Remove user from group
- `GET /api/tst/groups/{groupId}/users` - Get all users in a group
- `POST /api/tst/groups/{groupId}/users` - Add multiple users to a group

**Features:**
- Entity existence validation before relationship operations
- Bulk operations support
- Atomic operations (all-or-nothing)
- Idempotent delete operations

## Common Patterns

### Error Handling
All controllers implement consistent error handling:
- **200 OK** - Successful retrieval
- **201 Created** - Successful creation (with Location header)
- **204 No Content** - Successful update/delete
- **400 Bad Request** - Validation errors or malformed requests
- **404 Not Found** - Entity not found
- **500 Internal Server Error** - Unexpected server errors

### Documentation
- XML documentation comments on all public methods
- Swagger/OpenAPI integration ready
- Sample request/response examples in code comments
- Detailed remarks explaining validation rules and business logic

### Architecture
- **Service Layer** - Business logic and validation
- **Repository Layer** - Data access
- **Mapper** - DTO/Entity conversion
- **Validators** - Input validation
- Clean separation of concerns throughout

## Dependencies Required
Ensure the following are registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IUserService<TstUser>, TstUserService>();
builder.Services.AddScoped<IGroupService<TstGroup>, TstGroupService>();
builder.Services.AddScoped<IUserGroupRelationService<TstUser, TstGroup>, TstUserGroupRelationService>();
builder.Services.AddScoped<IUserRepository<TstUser>, TstUserRepository>();
builder.Services.AddScoped<IGroupRepository<TstGroup>, TstGroupRepository>();
builder.Services.AddScoped<IUserGroupRelationRepository, TstUserGroupRelationRepository>();
builder.Services.AddScoped<IValidator<TstUser>, TstUserValidator>();
builder.Services.AddScoped<IValidator<TstGroup>, TstGroupValidator>();
builder.Services.AddSingleton<TstMapper>();
```

## Testing
Access Swagger UI at the application root (configured in Program.cs) to:
- View all endpoints with full documentation
- Test API operations interactively
- View request/response schemas
- Examine validation rules

## Notes
- All operations are async for optimal performance
- Validation occurs at the service layer
- Controllers focus on HTTP concerns only
- Comprehensive inline documentation for maintainability

