# GrpcServer - User & Group Management API

A .NET 10.0 RESTful API for managing users and groups across multiple backend systems with full CRUD operations and many-to-many relationships.

## 📋 Table of Contents

- [Overview](#-overview)
- [Quick Start](#-quick-start)
- [API Endpoints](#-api-endpoints)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [Configuration](#-configuration)
- [Development Guide](#-development-guide)
- [Testing](#-testing)
- [Docker Deployment](#-docker-deployment)

---

## 🎯 Overview

GrpcServer is a multi-tenant API gateway that manages users and groups across different backend systems (ABC, INM, TST). Each system can have its own validation rules, data mappings, and repositories while sharing a common API interface.

**Key Technologies:**
- **.NET 10.0** - Modern C# framework
- **ASP.NET Core** - Web API framework
- **Swagger/OpenAPI** - Interactive API documentation
- **Keyed Dependency Injection** - Multi-system support
- **Docker** - Container deployment

**What You Can Do:**
- Create, read, update, and delete users and groups
- Manage user-group relationships (add/remove users to/from groups)
- Support multiple backend systems with different business rules
- Test all endpoints interactively via Swagger UI

---

## 🚀 Quick Start

### Prerequisites

- .NET 10.0 SDK or later ([download here](https://dotnet.microsoft.com/download/dotnet/10.0))
- (Optional) Docker for containerized deployment

### Run Locally in 3 Steps

1. **Navigate to the project directory:**
   ```bash
   cd GrpcDemo/GrpcServer
   ```

2. **Restore and run:**
   ```bash
   dotnet restore
   dotnet run
   ```

3. **Open Swagger UI in your browser:**
   - Navigate to `http://localhost:5185`
   - Start testing endpoints immediately!

The API will run on:
- **HTTP:** `http://localhost:5185`
- **HTTPS:** `https://localhost:7017`

### First API Call

Try creating a user:
```bash
curl -X POST http://localhost:5185/users \
  -H "Content-Type: application/json" \
  -d '{"userName": "johndoe", "email": "john@example.com"}'
```

---

## 🔌 API Endpoints

The API provides **18 RESTful endpoints** organized into three categories.

### 👥 Users (6 endpoints)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/users` | List all users |
| POST | `/users` | Create a new user |
| GET | `/users/{userId}` | Get user by ID |
| PUT | `/users/{userId}` | Update entire user |
| PATCH | `/users/{userId}` | Update specific fields |
| DELETE | `/users/{userId}` | Delete user |

**Example - Create User:**
```bash
POST /users
{
  "userName": "johndoe",
  "email": "john@example.com"
}
# Response: 201 Created
```

**Example - Partial Update:**
```bash
PATCH /users/1234
{
  "email": "newemail@example.com"
}
# Response: 200 OK
```

### 👪 Groups (6 endpoints)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/groups` | List all groups |
| POST | `/groups` | Create a new group |
| GET | `/groups/{groupId}` | Get group by ID |
| PUT | `/groups/{groupId}` | Update entire group |
| PATCH | `/groups/{groupId}` | Update specific fields |
| DELETE | `/groups/{groupId}` | Delete group |

**Example - Create Group:**
```bash
POST /groups
{
  "displayName": "Developers"
}
# Response: 201 Created
```

### 🔗 User-Group Relations (6 endpoints)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/users/{userId}/groups` | Get all groups for a user |
| POST | `/users/{userId}/groups` | Add user to multiple groups |
| DELETE | `/users/{userId}/groups/{groupId}` | Remove user from group |
| GET | `/groups/{groupId}/users` | Get all users in a group |
| POST | `/groups/{groupId}/users` | Add multiple users to group |
| DELETE | `/groups/{groupId}/users/{userId}` | Remove user from group |

**Example - Add User to Groups:**
```bash
POST /users/1234/groups
{
  "groupIds": [5678, 5679, 5680]
}
# Response: 204 No Content
```

### HTTP Response Codes

- **200 OK** - Successful GET, PUT, PATCH
- **201 Created** - Successful POST (new resource)
- **204 No Content** - Successful DELETE or relation change
- **400 Bad Request** - Validation failed
- **404 Not Found** - Resource doesn't exist
- **500 Internal Server Error** - Server error

---

## 🏗️ Architecture

The project follows a **layered architecture** with clear separation of concerns:

```
HTTP Request
     ↓
┌─────────────────────────────────┐
│  Controllers                    │  Handle HTTP requests/responses
│  - UsersController              │  Route to appropriate services
│  - GroupsController             │
│  - UserGroupRelationsController │
└─────────────────────────────────┘
     ↓
┌─────────────────────────────────┐
│  Services                       │  Business logic
│  - IUserService                 │  Coordinate operations
│  - IGroupService                │  Validate & process
│  - IUserGroupRelationService    │
└─────────────────────────────────┘
     ↓
┌─────────────────────────────────┐
│  Repositories                   │  Data access
│  - IUserRepository              │  CRUD operations
│  - IGroupRepository             │  External system calls
│  - IUserGroupRelationRepository │
└─────────────────────────────────┘
     ↓
Backend Systems (ABC, INM, TST)
```

**Supporting Components:**

- **DTOs** - Define API request/response contracts
- **Validators** - Enforce business rules per system
- **Mappers** - Convert between entities and DTOs
- **Models** - Domain entities (IBaseUser, IBaseGroup)

### Design Patterns

- **Repository Pattern** - Abstract data access
- **Strategy Pattern** - Pluggable validators/mappers per backend
- **Dependency Injection** - Keyed services for multi-system support
- **DTO Pattern** - Separate API contracts from domain models

---

## 📁 Project Structure

```
GrpcServer/
├── Program.cs                              # Application entry point & DI setup
├── GrpcServer.csproj                       # Project dependencies
├── Dockerfile                              # Container configuration
│
├── Infrastructure/
│   ├── Controllers/                        # API endpoints
│   │   ├── UsersController.cs
│   │   ├── GroupsController.cs
│   │   └── UserGroupRelationsController.cs
│   │
│   ├── Services/Common/                    # Business logic interfaces
│   │   ├── IUserService.cs
│   │   ├── IGroupService.cs
│   │   └── IUserGroupRelationService.cs
│   │
│   ├── Repositories/Common/                # Data access interfaces
│   │   ├── IUserRepository.cs
│   │   ├── IGroupRepository.cs
│   │   └── IUserGroupRelationRepository.cs
│   │
│   ├── Models/Common/                      # Domain entities
│   │   ├── IBaseUser.cs                   # User interface
│   │   ├── IBaseGroup.cs                  # Group interface
│   │   └── RelationDtos.cs                # Relationship DTOs
│   │
│   ├── Validators/Common/                  # Input validation
│   ├── Mappers/Common/                     # Entity-DTO mapping
│   │   └── IMapper.cs
│   │
│   ├── Enum/
│   │   └── AppCode.cs                     # System codes (Inm, Abc, Tst)
│   │
│   └── Settings/                           # Configuration files
│       ├── appsettings.json
│       └── appsettings.Development.json
│
└── Documentation/                          # Project docs
    └── COMPREHENSIVE_README.md             # This file
```

### Key Files

- **Program.cs** - Configures services, middleware, and DI
- **AppCode.cs** - Enum defining backend systems (Inm, Abc, Tst)
- **Controllers/** - RESTful API endpoints with routing
- **Services/** - Business logic layer
- **Repositories/** - Data access abstractions
- **Models/** - Domain entity interfaces

---

## ⚙️ Configuration

### Application Codes

The system supports multiple backend systems via the `AppCode` enum:

```csharp
public enum AppCode
{
    Inm,  // In-Memory system
    Abc,  // ABC external system
    Tst   // Test system
}
```

Each system can have its own:
- Validation rules
- Data mappings
- Repository implementations
- Business logic

### Port Configuration

Edit `Properties/launchSettings.json` to change ports:

```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5185"
    },
    "https": {
      "applicationUrl": "https://localhost:7017;http://localhost:5185"
    }
  }
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Set to `Development`, `Staging`, or `Production`
- `ASPNETCORE_URLS` - Override default URLs

### Dependencies

Key NuGet packages:
- `Grpc.AspNetCore` (2.64.0) - gRPC support
- `Swashbuckle.AspNetCore` (6.5.0) - Swagger/OpenAPI

---

## 🔧 Development Guide

### Dependency Injection Pattern

The project uses **keyed dependency injection** to support multiple backend systems with different implementations.

#### Registering Services

Each `AppCode` gets its own services in `Program.cs`:

```csharp
// Register system-specific mapper
builder.Services.AddKeyedSingleton<IMapper<AbcUser, AbcUserDto>>(
    AppCode.Abc, 
    new AbcMapper()
);

// Register system-specific validator
builder.Services.AddKeyedSingleton<IUserValidator>(
    AppCode.Abc,
    new AbcUserValidator()
);
```

#### Using Keyed Services

Inject services by key in controllers:

```csharp
[ApiController]
[Route("/api/v1/abc/users")]
public class AbcUsersController : ControllerBase
{
    private readonly IMapper<AbcUser, AbcUserDto> _mapper;
    private readonly IUserValidator _validator;

    public AbcUsersController(
        [FromKeyedServices(AppCode.Abc)] IMapper<AbcUser, AbcUserDto> mapper,
        [FromKeyedServices(AppCode.Abc)] IUserValidator validator)
    {
        _mapper = mapper;
        _validator = validator;
    }
}
```

### Adding a New Backend System

Follow these 5 steps:

**1. Add AppCode Enum Value**
```csharp
public enum AppCode
{
    Inm, Abc, Tst,
    Xyz  // New system
}
```

**2. Create Domain Models**
```csharp
public class XyzUser : IBaseUser 
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class XyzGroup : IBaseGroup 
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
}
```

**3. Implement Mapper**
```csharp
public class XyzMapper : IMapper<XyzUser, XyzUserDto>
{
    public XyzUserDto ToDto(XyzUser entity) { /* ... */ }
    public XyzUser FromDto(XyzUserDto dto) { /* ... */ }
    public void ApplyPatch(XyzUser entity, XyzUserDto dto) { /* ... */ }
}
```

**4. Implement Validator**
```csharp
public class XyzUserValidator : IUserValidator
{
    public bool IsValid(IBaseUser user)
    {
        // Add XYZ-specific validation rules
        return !string.IsNullOrWhiteSpace(user.UserName) &&
               !string.IsNullOrWhiteSpace(user.Email);
    }
}
```

**5. Register in Program.cs**
```csharp
builder.Services.AddKeyedSingleton<IMapper<XyzUser, XyzUserDto>>(
    AppCode.Xyz, new XyzMapper());
builder.Services.AddKeyedSingleton<IUserValidator>(
    AppCode.Xyz, new XyzUserValidator());
```

### Validation Pattern

All validators implement `IValidator<T>`:

```csharp
public interface IValidator<T>
{
    bool IsValid(T entity);
}

public interface IUserValidator : IValidator<IBaseUser> { }
public interface IGroupValidator : IValidator<IBaseGroup> { }
```

### Mapper Pattern

Mappers handle entity-DTO conversions:

```csharp
public interface IMapper<TEntity, TDto>
{
    TDto ToDto(TEntity entity);
    TEntity FromDto(TDto dto);
    void ApplyPatch(TEntity entity, TDto patchDto);
}
```

---

## 🧪 Testing

### Using Swagger UI

The easiest way to test the API:

1. **Start the application:**
   ```bash
   dotnet run
   ```

2. **Open Swagger UI:**
   - Navigate to `http://localhost:5185`

3. **Test an endpoint:**
   - Click any endpoint to expand it
   - Click "Try it out"
   - Fill in the request body/parameters
   - Click "Execute"
   - View the response

### Sample Test Workflow

Complete user lifecycle test:

```bash
# 1. Create a user
POST /users
{
  "userName": "testuser",
  "email": "test@example.com"
}
# Save the returned userId

# 2. Create a group
POST /groups
{
  "displayName": "Test Group"
}
# Save the returned groupId

# 3. Add user to group
POST /users/{userId}/groups
{
  "groupIds": ["{groupId}"]
}

# 4. Verify relationship
GET /users/{userId}/groups
# Should return the group

# 5. Update user
PATCH /users/{userId}
{
  "email": "updated@example.com"
}

# 6. Clean up
DELETE /users/{userId}
DELETE /groups/{groupId}
```

### Unit Tests

Run the test project:

```bash
cd ../GrpcServer.Tests
dotnet test
```

The test project includes:
- Repository tests
- Service tests
- Validator tests
- Mapper tests

---

## 🐳 Docker Deployment

### Quick Start with Docker

**Build the image:**
```bash
docker build -t grpcserver:latest .
```

**Run the container:**
```bash
docker run -p 8080:8080 -p 8081:8081 grpcserver:latest
```

**Access the API:**
- HTTP: `http://localhost:8080`
- HTTPS: `http://localhost:8081`

### Docker Compose

Use the solution's `compose.yaml` for multi-container setup:

```bash
cd ../
docker-compose up
```

### Production Deployment

Set environment variables:

```bash
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS="http://+:8080" \
  grpcserver:latest
```

The Dockerfile uses multi-stage builds for optimization:
- **Base:** .NET 10.0 runtime
- **Build:** SDK for compilation
- **Publish:** Optimized output
- **Final:** Minimal production image

---

## 📊 Project Status

### ✅ Implemented

- 18 RESTful API endpoints (Users, Groups, Relations)
- Layered architecture (Controllers, Services, Repositories)
- DTO pattern for all operations
- Keyed dependency injection
- Pluggable validators and mappers
- Swagger/OpenAPI documentation
- Docker support
- Comprehensive documentation

### 🚧 In Progress

- Repository implementations (external API integration)
- Persistent storage for user-group relations
- Authentication and authorization
- Logging and monitoring
- Integration tests

### 🎯 Future Enhancements

- gRPC service implementations
- Health check endpoints
- Rate limiting
- Caching layer
- Database integration
- Message queue support
- Distributed tracing

---

## 📚 Additional Resources

- **Swagger UI** - `http://localhost:5185` (interactive API testing)
- **Project Documentation** - See `/Documentation` folder
- **.NET 10.0 Docs** - https://docs.microsoft.com/dotnet/
- **ASP.NET Core** - https://docs.microsoft.com/aspnet/core/

---

## 💡 Tips & Best Practices

- Always validate input using the validator pattern
- Use keyed services for system-specific implementations
- Follow the DTO pattern for API contracts
- Write unit tests for new validators and mappers
- Update Swagger comments for new endpoints
- Use PATCH for partial updates, PUT for full replacement
- Check Swagger UI for schema validation requirements

---

**Built with .NET 10.0 | RESTful API | Swagger/OpenAPI | Docker**

