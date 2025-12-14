dd# User & Group Management API

A RESTful CRUD API for managing Users and Groups with many-to-many relationships.

## Features

- ✅ Full CRUD operations for Users
- ✅ Full CRUD operations for Groups
- ✅ Many-to-many relationship management between Users and Groups
- ✅ RESTful API design with proper HTTP verbs (GET, POST, PUT, PATCH, DELETE)
- ✅ Request/Response DTOs
- ✅ Input validation
- ✅ OpenAPI/Swagger documentation
- ✅ Swagger UI for interactive API testing

## Architecture

The project follows a clean, layered architecture:

```
Controllers/          # API endpoints (HTTP layer)
├── UsersController.cs
├── GroupsController.cs
└── UserGroupRelationsController.cs

DTOs/                 # Data Transfer Objects
├── UserDtos.cs       # UserDto, CreateUserDto, UpdateUserDto, PatchUserDto
├── GroupDtos.cs      # GroupDto, CreateGroupDto, UpdateGroupDto, PatchGroupDto
└── RelationDtos.cs   # AddUserToGroupsDto, AddUsersToGroupDto

Services/             # Business logic layer
├── UserService.cs
├── GroupService.cs
├── IUserGroupRelationService.cs
└── UserGroupRelationService.cs

Repositories/         # Data access layer (API-based)
├── IUserRepository.cs
├── IGroupRepository.cs
├── MckUserRepository.cs
└── MckGroupRepository.cs

Mappers/              # Entity-DTO mapping
├── UserMapper.cs
└── GroupMapper.cs

Validators/           # Input validation
├── UserValidator.cs
└── GroupValidator.cs

Models/               # Domain entities
├── IUser.cs
├── IGroup.cs
├── MckUser.cs
└── MckGroup.cs
```

## API Endpoints

### Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/users` | List all users |
| POST | `/users` | Create a new user |
| GET | `/users/{userId}` | Get a specific user |
| PUT | `/users/{userId}` | Replace a user (full update) |
| PATCH | `/users/{userId}` | Partially update a user |
| DELETE | `/users/{userId}` | Delete a user |

### Groups

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/groups` | List all groups |
| POST | `/groups` | Create a new group |
| GET | `/groups/{groupId}` | Get a specific group |
| PUT | `/groups/{groupId}` | Replace a group (full update) |
| PATCH | `/groups/{groupId}` | Partially update a group |
| DELETE | `/groups/{groupId}` | Delete a group |

### User-Group Relations

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/users/{userId}/groups` | Get all groups for a user |
| POST | `/users/{userId}/groups` | Add user to multiple groups |
| DELETE | `/users/{userId}/groups/{groupId}` | Remove user from a group |
| GET | `/groups/{groupId}/users` | Get all users in a group |
| POST | `/groups/{groupId}/users` | Add multiple users to a group |
| DELETE | `/groups/{groupId}/users/{userId}` | Remove user from a group |

## Running the Application

### Prerequisites

- .NET 9.0 SDK

### Start the Server

```bash
cd GrpcServer
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5001`

### Access Swagger UI

Once the application is running, navigate to:
- **Swagger UI**: `http://localhost:5001` or `https://localhost:7001`

This will open an interactive API documentation where you can test all endpoints.

## Example Requests

### Create a User

```bash
curl -X POST https://localhost:7001/users \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "johndoe",
    "email": "john@example.com"
  }'
```

### Create a Group

```bash
curl -X POST https://localhost:7001/groups \
  -H "Content-Type: application/json" \
  -d '{
    "displayName": "Administrators"
  }'
```

### Add User to Groups

```bash
curl -X POST https://localhost:7001/users/1/groups \
  -H "Content-Type: application/json" \
  -d '{
    "groupIds": [1, 2, 3]
  }'
```

### Get User's Groups

```bash
curl https://localhost:7001/users/1/groups
```

### Partial Update User

```bash
curl -X PATCH https://localhost:7001/users/1 \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newemail@example.com"
  }'
```

## Validation Rules

### User Validation

- **userName**: Required, minimum 3 characters
- **email**: Required, must be a valid email format

### Group Validation

- **displayName**: Required, minimum 3 characters

## Response Codes

- **200 OK**: Successful GET, PUT, or PATCH request
- **201 Created**: Successful POST request (resource created)
- **204 No Content**: Successful DELETE or relation operation
- **400 Bad Request**: Validation error or malformed request
- **404 Not Found**: Resource not found

## OpenAPI Specification

The OpenAPI specification is available at:
- **Swagger JSON**: `https://localhost:7001/swagger/v1/swagger.json`
- **Static YAML**: `openapi.yaml` (in project root)

## Current Implementation Status

- ✅ Complete API structure with all endpoints
- ✅ DTOs for request/response
- ✅ Validators with business rules
- ✅ Mappers for entity-DTO conversion
- ✅ Service layer architecture
- ✅ Repository pattern for data access
- ✅ Swagger/OpenAPI documentation
- ⚠️  Repository implementations use stub/mock logic (ready for external API integration)
- ⚠️  User-Group relations use in-memory storage (mock implementation)

## Next Steps

To connect to actual external APIs:

1. Update `MckUserRepository.cs` and `MckGroupRepository.cs` with actual HttpClient calls
2. Configure base URLs in `appsettings.json`
3. Add authentication/authorization if needed
4. Replace in-memory relation storage with actual backend storage or API calls

## Development

### Add New Endpoints

1. Create DTO in `/DTOs`
2. Add validation in `/Validators`
3. Create mapper in `/Mappers`
4. Add controller action in `/Controllers`

### Testing with Swagger UI

1. Run the application
2. Open browser to `http://localhost:5001`
3. Expand any endpoint
4. Click "Try it out"
5. Fill in parameters/request body
6. Click "Execute"
7. View response

## License

This is a demonstration project.

