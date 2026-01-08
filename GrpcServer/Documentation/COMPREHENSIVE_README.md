# GrpcServer - User & Group Management API

A comprehensive .NET 9.0 RESTful API for managing Users and Groups across multiple backend systems with full CRUD operations and many-to-many relationship support.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Configuration](#configuration)
- [Dependency Injection](#dependency-injection)
- [Development](#development)
- [Docker Support](#docker-support)
- [Testing](#testing)

---

## 🎯 Overview

GrpcServer is a multi-tenant API gateway that provides a unified interface for managing users and groups across different backend systems. It supports multiple application codes (ABC, INM, TST) with pluggable validators, mappers, and repositories.

**Technology Stack:**
- **.NET 9.0** - Target framework
- **ASP.NET Core** - Web framework
- **gRPC** - High-performance RPC framework (configured)
- **Swagger/OpenAPI** - API documentation and testing
- **Dependency Injection** - Keyed services pattern

---

## ✨ Features

- ✅ **Full CRUD Operations** for Users and Groups
- ✅ **Many-to-Many Relationships** between Users and Groups
- ✅ **Multi-System Support** with application-specific implementations (ABC, INM, TST)
- ✅ **RESTful API Design** with proper HTTP verbs and status codes
- ✅ **Keyed Dependency Injection** for system-specific services
- ✅ **Request/Response DTOs** for clean API contracts
- ✅ **Input Validation** with pluggable validators
- ✅ **Entity-DTO Mapping** with automatic conversions
- ✅ **OpenAPI/Swagger Documentation** with interactive UI
- ✅ **Partial Updates** via PATCH operations
- ✅ **Docker Support** for containerized deployments
- ✅ **Extensible Architecture** for adding new systems

---

## 🏗️ Architecture

The project follows a **clean, layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────┐
│   Controllers (HTTP Endpoints)              │
│   - Route definitions                       │
│   - Request/Response handling               │
├─────────────────────────────────────────────┤
│   DTOs + Validators + Mappers               │
│   - Data contracts                          │
│   - Validation rules                        │
│   - Entity transformations                  │
├─────────────────────────────────────────────┤
│   Services (Business Logic)                 │
│   - User service                            │
│   - Group service                           │
│   - Relation service                        │
├─────────────────────────────────────────────┤
│   Repositories (Data Access)                │
│   - Abstract interfaces                     │
│   - System-specific implementations         │
├─────────────────────────────────────────────┤
│   Models (Domain Entities)                  │
│   - IBaseUser / IBaseGroup interfaces       │
│   - System-specific models                  │
└─────────────────────────────────────────────┘
```

### Core Design Patterns

1. **Repository Pattern** - Data access abstraction
2. **Dependency Injection** - Keyed services for multi-system support
3. **DTO Pattern** - Separation of API contracts from domain models
4. **Mapper Pattern** - Clean entity-DTO transformations
5. **Strategy Pattern** - Pluggable validators and mappers per system

---

## 📁 Project Structure

```
GrpcServer/
├── Program.cs                      # Application entry point & DI configuration
├── GrpcServer.csproj              # Project file with dependencies
├── Dockerfile                     # Container configuration
├── Properties/
│   └── launchSettings.json        # Development launch profiles
│
├── Infrastructure/                # Core application code
│   ├── Controllers/               # API endpoints (REST controllers)
│   │   ├── UsersController.cs
│   │   ├── GroupsController.cs
│   │   └── UserGroupRelationsController.cs
│   │
│   ├── Enum/
│   │   └── AppCode.cs            # System identifiers (Inm, Abc, Tst)
│   │
│   ├── Models/
│   │   └── Common/
│   │       ├── IBaseUser.cs      # User interface (Id, UserName, Email)
│   │       ├── IBaseGroup.cs     # Group interface (Id, DisplayName)
│   │       └── RelationDtos.cs   # Relationship DTOs
│   │
│   ├── Repositories/
│   │   └── Common/
│   │       ├── IUserRepository.cs           # User data access interface
│   │       ├── IGroupRepository.cs          # Group data access interface
│   │       └── IUserGroupRelationRepository.cs
│   │
│   ├── Services/
│   │   └── Common/
│   │       ├── IUserService.cs              # User business logic interface
│   │       ├── IGroupService.cs             # Group business logic interface
│   │       └── IUserGroupRelationService.cs # Relation management
│   │
│   ├── Validators/                # Input validation (keyed by AppCode)
│   │   └── Common/
│   │
│   ├── Mappers/                   # Entity-DTO mapping (keyed by AppCode)
│   │   └── Common/
│   │       └── IMapper.cs        # Generic mapper interface
│   │
│   └── Settings/                  # Configuration models
│
└── Documentation/                 # Project documentation
    ├── API_README.md             # API usage guide
    ├── QUICKSTART.md             # Quick start guide
    ├── IMPLEMENTATION_SUMMARY.md  # Implementation details
    ├── DI_REGISTRATION_GUIDE.md   # DI patterns and examples
    └── *.md                       # Additional documentation
```

---

## 🚀 Getting Started

### Prerequisites

- **.NET 9.0 SDK** or later
- **Docker** (optional, for containerized deployment)

### Installation

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd GrpcDemo/GrpcServer
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

### Running the Application

#### Development Mode

```bash
dotnet run
```

The API will be available at:
- **HTTP:** `http://localhost:5185`
- **HTTPS:** `https://localhost:7017`

#### Docker

```bash
# Build image
docker build -t grpcserver:latest .

# Run container
docker run -p 8080:8080 -p 8081:8081 grpcserver:latest
```

### Accessing Swagger UI

Once running, open your browser to:
- **Swagger UI:** `http://localhost:5185` (or configured port)

The interactive Swagger UI allows you to:
- Browse all 18 API endpoints
- Test endpoints directly in the browser
- View request/response schemas
- See validation requirements

---

## 🔌 API Endpoints

### Total: 18 Endpoints (6 Users + 6 Groups + 6 Relations)

### 👥 Users API

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/users` | List all users | - | Array of UserDto |
| POST | `/users` | Create user | CreateUserDto | UserDto (201) |
| GET | `/users/{userId}` | Get user by ID | - | UserDto |
| PUT | `/users/{userId}` | Replace user | UpdateUserDto | UserDto |
| PATCH | `/users/{userId}` | Partial update | PatchUserDto | UserDto |
| DELETE | `/users/{userId}` | Delete user | - | 204 No Content |

### 👪 Groups API

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/groups` | List all groups | - | Array of GroupDto |
| POST | `/groups` | Create group | CreateGroupDto | GroupDto (201) |
| GET | `/groups/{groupId}` | Get group by ID | - | GroupDto |
| PUT | `/groups/{groupId}` | Replace group | UpdateGroupDto | GroupDto |
| PATCH | `/groups/{groupId}` | Partial update | PatchGroupDto | GroupDto |
| DELETE | `/groups/{groupId}` | Delete group | - | 204 No Content |

### 🔗 User-Group Relations API

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/users/{userId}/groups` | Get user's groups | - | Array of GroupDto |
| POST | `/users/{userId}/groups` | Add user to groups | AddUserToGroupsDto | 204 No Content |
| DELETE | `/users/{userId}/groups/{groupId}` | Remove user from group | - | 204 No Content |
| GET | `/groups/{groupId}/users` | Get group's users | - | Array of UserDto |
| POST | `/groups/{groupId}/users` | Add users to group | AddUsersToGroupDto | 204 No Content |
| DELETE | `/groups/{groupId}/users/{userId}` | Remove user from group | - | 204 No Content |

### Example Requests

#### Create a User
```bash
curl -X POST http://localhost:5185/users \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "johndoe",
    "email": "john@example.com"
  }'
```

#### Add User to Multiple Groups
```bash
curl -X POST http://localhost:5185/users/1234/groups \
  -H "Content-Type: application/json" \
  -d '{
    "groupIds": [5678, 5679]
  }'
```

#### Partial Update (PATCH)
```bash
curl -X PATCH http://localhost:5185/users/1234 \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newemail@example.com"
  }'
```

### Response Codes

- **200 OK** - Successful GET, PUT, or PATCH
- **201 Created** - Successful POST (resource created)
- **204 No Content** - Successful DELETE or relation operation
- **400 Bad Request** - Validation error or malformed request
- **404 Not Found** - Resource not found
- **501 Not Implemented** - Operation not yet supported

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

### Launch Settings

Configure in `Properties/launchSettings.json`:

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

### NuGet Dependencies

```xml
<PackageReference Include="Grpc.AspNetCore" Version="2.64.0"/>
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0"/>
```

---

## 🔧 Dependency Injection

The project uses **keyed dependency injection** to support multiple backend systems with different implementations.

### Keyed Services Pattern

Each `AppCode` has its own registered services:

```csharp
// Registering system-specific mappers
builder.Services.AddKeyedSingleton<IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto>>(
    AppCode.Abc, 
    new AbcMapper()
);

// Registering system-specific validators
builder.Services.AddKeyedSingleton<IUserValidator>(
    AppCode.Abc,
    new AbcUserValidator()
);
```

### Using Keyed Services in Controllers

```csharp
[ApiController]
[Route("/api/v1/abc/users")]
public class AbcUsersController : ControllerBase
{
    private readonly IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> _mapper;
    private readonly IUserValidator _validator;

    public AbcUsersController(
        [FromKeyedServices(AppCode.Abc)] IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> mapper,
        [FromKeyedServices(AppCode.Abc)] IUserValidator validator)
    {
        _mapper = mapper;
        _validator = validator;
    }
    
    // Controller actions...
}
```

### Benefits

1. **Type Safety** - Each system gets its own implementation
2. **Clear Separation** - Different validation/mapping logic per system
3. **Easy Testing** - Mock specific implementations by key
4. **Scalability** - Add new systems without conflicts

---

## 💻 Development

### Adding a New Backend System

1. **Add AppCode enum value:**
   ```csharp
   public enum AppCode
   {
       Inm, Abc, Tst,
       Xyz  // New system
   }
   ```

2. **Create system-specific models:**
   ```csharp
   public class XyzUser : IBaseUser { /* ... */ }
   public class XyzGroup : IBaseGroup { /* ... */ }
   ```

3. **Implement mappers and validators:**
   ```csharp
   public class XyzMapper : IMapper<XyzUser, XyzUserRequestDto, XyzUserResponseDto> { }
   public class XyzUserValidator : IUserValidator { }
   ```

4. **Register in Program.cs:**
   ```csharp
   builder.Services.AddKeyedSingleton<IMapper<...>>(AppCode.Xyz, new XyzMapper());
   builder.Services.AddKeyedSingleton<IUserValidator>(AppCode.Xyz, new XyzUserValidator());
   ```

5. **Create controller:**
   ```csharp
   [ApiController]
   [Route("/api/v1/xyz/users")]
   public class XyzUsersController : ControllerBase { }
   ```

### Validation Rules

Validators implement `IValidator<T>` and define business rules:

```csharp
public interface IUserValidator : IValidator<IBaseUser> { }

public class MyUserValidator : IUserValidator
{
    public bool IsValid(IBaseUser entity)
    {
        return !string.IsNullOrWhiteSpace(entity.UserName) &&
               !string.IsNullOrWhiteSpace(entity.Email) &&
               entity.Email.Contains("@");
    }
}
```

### Mapper Pattern

Mappers handle entity-DTO conversions:

```csharp
public interface IMapper<TEntity, TRequestDto, TResponseDto>
{
    TResponseDto ToResponseDto(TEntity entity);
    TEntity FromRequestDto(TRequestDto dto);
    void ApplyPatch(TEntity entity, TRequestDto dto);
}
```

---

## 🐳 Docker Support

### Dockerfile Overview

The project includes a multi-stage Dockerfile:

- **Base Stage:** ASP.NET 9.0 runtime
- **Build Stage:** SDK for compilation
- **Publish Stage:** Optimized output
- **Final Stage:** Minimal runtime image

### Docker Commands

```bash
# Build
docker build -t grpcserver:latest .

# Run
docker run -p 8080:8080 -p 8081:8081 grpcserver:latest

# Run with environment variables
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  grpcserver:latest
```

### Docker Compose

See `compose.yaml` in the solution root for orchestration configuration.

---

## 🧪 Testing

### Manual Testing with Swagger UI

1. **Start the application** (`dotnet run`)
2. **Open Swagger UI** (http://localhost:5185)
3. **Select an endpoint** and click "Try it out"
4. **Fill in parameters** and request body
5. **Click "Execute"** and view the response

### Example Test Workflow

1. **Create User** → POST `/users` → Get userId from response
2. **Create Group** → POST `/groups` → Get groupId from response
3. **Add to Group** → POST `/users/{userId}/groups` with groupIds
4. **Verify Relation** → GET `/users/{userId}/groups`
5. **Update User** → PATCH `/users/{userId}` with new data
6. **Delete User** → DELETE `/users/{userId}`

### Unit Testing

The solution includes a test project:
```
GrpcServer.Tests/
├── Tests/
│   ├── Repositories/
│   ├── Services/
│   └── Validators/
```

Run tests:
```bash
cd GrpcServer.Tests
dotnet test
```

---

## 📚 Additional Documentation

For more detailed information, see:

- **[API_README.md](Documentation/API_README.md)** - Comprehensive API guide with examples
- **[QUICKSTART.md](Documentation/QUICKSTART.md)** - Quick start guide for immediate testing
- **[IMPLEMENTATION_SUMMARY.md](Documentation/IMPLEMENTATION_SUMMARY.md)** - Implementation details and status
- **[DI_REGISTRATION_GUIDE.md](Documentation/DI_REGISTRATION_GUIDE.md)** - Dependency injection patterns
- **[MULTI_CONTROLLER_IMPLEMENTATION.md](Documentation/MULTI_CONTROLLER_IMPLEMENTATION.md)** - Controller architecture
- **[DI_MAPPER_VALIDATOR_IMPLEMENTATION.md](Documentation/DI_MAPPER_VALIDATOR_IMPLEMENTATION.md)** - Mapper and validator details

---

## 🎯 Current Status

### ✅ Completed

- Full RESTful API with 18 endpoints
- Controllers for Users, Groups, and Relations
- DTO pattern for all requests/responses
- Validation framework with pluggable validators
- Mapper framework for entity-DTO transformations
- Keyed dependency injection setup
- OpenAPI/Swagger documentation
- Docker support
- Comprehensive documentation

### ⚠️ In Progress

- Repository implementations (currently stubs for external API integration)
- User-Group relation storage (currently in-memory mock)
- Health check endpoints
- Authentication/Authorization
- Logging and monitoring
- Integration tests

### 🔮 Future Enhancements

- gRPC endpoint implementations
- Rate limiting
- Caching layer
- Database persistence
- Message queue integration
- Distributed tracing

---

## 🤝 Contributing

When adding new features:

1. Follow the existing architecture patterns
2. Add appropriate documentation
3. Include XML comments for Swagger
4. Write unit tests
5. Update this README if needed

---

## 📄 License

This is a demonstration project.

---

## 📞 Support

For questions or issues:
- Check the documentation in the `/Documentation` folder
- Review the Swagger UI for API details
- Examine the code comments and XML documentation

---

**Built with .NET 9.0 | RESTful API | Swagger/OpenAPI | Docker**

