# GrpcServer.Tests

## Overview
This is the test project for **GrpcServer**, providing comprehensive unit tests for a multi-tenant user and group management system. The tests demonstrate the proper implementation and usage of the TST (Test) application code domain with in-memory repositories, services, validators, and mappers.

## Technology Stack
- **.NET 9.0**
- **xUnit** - Testing framework
- **Moq** - Mocking library
- **Coverlet** - Code coverage tool

## Project Structure

```
GrpcServer.Tests/
├── Infrastructure/               # Test implementation of TST domain
│   ├── Mappers/
│   │   └── TST/
│   │       └── TstMapper.cs     # Maps between entities and DTOs
│   ├── Models/
│   │   └── TST/
│   │       ├── TstUser.cs       # User entity with TST-specific extensions
│   │       ├── TstGroup.cs      # Group entity with TST-specific extensions
│   │       └── DTO/             # Data Transfer Objects
│   │           ├── TstUserRequestDto.cs
│   │           ├── TstUserResponseDto.cs
│   │           ├── TstGroupRequestDto.cs
│   │           └── TstGroupResponseDto.cs
│   ├── Repositories/
│   │   └── TST/
│   │       ├── TstUserRepository.cs           # In-memory user storage
│   │       ├── TstGroupRepository.cs          # In-memory group storage
│   │       └── TstUserGroupRelationRepository.cs  # In-memory relationship storage
│   ├── Services/
│   │   └── TST/
│   │       ├── TstUserService.cs              # User business logic
│   │       ├── TstGroupService.cs             # Group business logic
│   │       └── TstUserGroupRelationService.cs # User-Group relationship logic
│   └── Validators/
│       └── TST/
│           ├── TstUserValidator.cs            # User validation rules
│           └── TstGroupValidator.cs           # Group validation rules
└── Tests/                        # Unit tests
    ├── Repositories/
    │   └── TST/
    │       ├── TstUserRepositoryTests.cs
    │       ├── TstGroupRepositoryTests.cs
    │       └── TstUserGroupRelationRepositoryTests.cs
    └── Services/
        └── TST/
            ├── TstUserServiceTests.cs
            ├── TstGroupServiceTests.cs
            └── TstUserGroupRelationServiceTests.cs
```

## Domain Models

### TstUser
User entity with TST-specific extensions:
- **Id** - Unique identifier
- **UserName** - User's login name
- **Email** - User's email address
- **TstUserExtension1** - Custom TST field (required, cannot be "forbidden")
- **TstUserExtension2** - Custom TST field (optional)

### TstGroup
Group entity with TST-specific extensions:
- **Id** - Unique identifier
- **DisplayName** - Group display name
- **TstGroupExtension1** - Custom TST field (required, min 5 characters)
- **TstGroupExtension2** - Custom TST field (optional)

## Business Logic

### TstUserService
- **Email Normalization**: Automatically converts all emails to lowercase
- **Validation**: Enforces user validation rules before add/update operations
- Throws `ArgumentException` for invalid or non-TST users

### TstGroupService
- **Display Name Trimming**: Automatically trims whitespace from display names
- **Validation**: Enforces group validation rules before add/update operations
- Throws `ArgumentException` for invalid or non-TST groups

### TstUserGroupRelationService
- **Relationship Management**: Manages many-to-many relationships between users and groups
- **Referential Integrity**: Validates that both users and groups exist before creating relationships
- **App Code**: Returns `AppCode.Tst` for TST domain operations
- Throws `InvalidOperationException` when users or groups don't exist

## Validation Rules

### TstUserValidator
- Id must not be empty or whitespace
- UserName must not be empty or whitespace
- Email must not be empty and must contain '@'
- TstUserExtension1 must not equal "forbidden"

### TstGroupValidator
- Id must not be empty or whitespace
- DisplayName must not be empty or whitespace
- TstGroupExtension1 must not be empty and must be at least 5 characters long

## Repository Implementation

All repositories use **in-memory dictionaries** for testing purposes:

### TstUserRepository
- Stores users in `Dictionary<string, TstUser>`
- Enforces unique user IDs
- Type-checks to ensure only TstUser instances are stored

### TstGroupRepository
- Stores groups in `Dictionary<string, TstGroup>`
- Enforces unique group IDs
- Type-checks to ensure only TstGroup instances are stored

### TstUserGroupRelationRepository
- Stores relationships in `HashSet<(string UserId, string GroupId)>`
- Prevents duplicate relationships
- Supports bidirectional queries (users by group, groups by user)

## Mapper Implementation

### TstMapper
Implements `IMapper<TEntity, TRequestDto, TResponseDto>` for both User and Group:

**User Mapping**:
- `ToResponseDto(TstUser)` → `TstUserResponseDto`
- `FromRequestDto(TstUserRequestDto)` → `TstUser`
- `ApplyPatch(TstUser, TstUserRequestDto)` - Updates entity from DTO

**Group Mapping**:
- `ToResponseDto(TstGroup)` → `TstGroupResponseDto`
- `FromRequestDto(TstGroupRequestDto)` → `TstGroup`
- `ApplyPatch(TstGroup, TstGroupRequestDto)` - Updates entity from DTO

## Test Coverage

### Repository Tests (184-238 tests each)
✅ Add, Update, Delete, Get operations  
✅ Type validation (only TST types accepted)  
✅ Duplicate ID prevention  
✅ Non-existent entity handling  
✅ TST-specific property storage and retrieval  

### Service Tests (340-383 tests each)
✅ Business logic validation (email normalization, name trimming)  
✅ Validator integration  
✅ Type checking for TST entities  
✅ Invalid entity handling  
✅ Relationship management (user-group relations)  
✅ Referential integrity checks  

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~TstUserServiceTests"
```

## Key Testing Patterns

### Arrange-Act-Assert Pattern
All tests follow the AAA pattern:
```csharp
[Fact]
public async Task GetByIdAsync_WithValidUser_ReturnsUser()
{
    // Arrange - Setup test data and dependencies
    var repository = new TstUserRepository();
    var validator = new TstUserValidator();
    var service = new TstUserService(repository, validator);
    
    // Act - Execute the method under test
    var result = await service.GetByIdAsync("user1");
    
    // Assert - Verify the results
    Assert.NotNull(result);
    Assert.Equal("user1", result.Id);
}
```

### In-Memory Testing
All repositories use in-memory collections, eliminating external dependencies and ensuring fast, isolated tests.

### Exception Testing
Validates proper exception handling:
```csharp
var exception = await Assert.ThrowsAsync<ArgumentException>(
    async () => await service.AddAsync(invalidUser)
);
Assert.Contains("validation failed", exception.Message);
```

## Dependencies
- **GrpcServer** - Main project reference for interfaces and base models
- **Microsoft.NET.Test.Sdk** (17.12.0)
- **xUnit** (2.9.2)
- **xUnit.runner.visualstudio** (2.8.2)
- **Moq** (4.20.72)
- **coverlet.collector** (6.0.2)

## Notes
- All repositories are **in-memory** implementations for testing purposes only
- The TST domain demonstrates the pattern for implementing application-specific logic
- Tests validate both success and failure scenarios
- Type safety is enforced at the service and repository levels
- Business logic (normalization, trimming) is applied consistently across operations

## Related Documentation
See the **GrpcServer** project for:
- Interface definitions (`IUserService`, `IGroupService`, etc.)
- Base model interfaces (`IBaseUser`, `IBaseGroup`)
- Common repository interfaces
- Validator interfaces
- Mapper interfaces

