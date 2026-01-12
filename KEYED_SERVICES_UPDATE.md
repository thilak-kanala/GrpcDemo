# Keyed Services Update

## Summary
All services in the GrpcServer project have been updated to use keyed services with `AppCode.TST` as the key for all TST-related services.

## Changes Made

### 1. Program.cs
- Updated all service registrations to use keyed services:
  - `AddKeyedScoped` for services (UserService, GroupService, UserGroupRelationService)
  - `AddKeyedSingleton` for repositories (UserRepository, GroupRepository, UserGroupRelationRepository)
  - `AddKeyedScoped` for validators (TstUserValidator, TstGroupValidator)
  - `AddKeyedSingleton` for mappers (TstMapper)
  - `AddKeyedScoped` for data seeder (TstDataSeeder)
- Updated seeder instantiation to use `GetRequiredKeyedService<TstDataSeeder>(AppCode.TST)`
- Added using statement: `using GrpcServer.Infrastructure.Enum;`

### 2. Controllers
All three controllers updated with `[FromKeyedServices(AppCode.TST)]` attribute:

#### TstUserController.cs
- Added using statements: `GrpcServer.Infrastructure.Enum` and `Microsoft.Extensions.DependencyInjection`
- Updated constructor parameters with `[FromKeyedServices(AppCode.TST)]` for:
  - `IUserService<TstUser>`
  - `TstMapper`

#### TstGroupController.cs
- Added using statements: `GrpcServer.Infrastructure.Enum` and `Microsoft.Extensions.DependencyInjection`
- Updated constructor parameters with `[FromKeyedServices(AppCode.TST)]` for:
  - `IGroupService<TstGroup>`
  - `TstMapper`

#### TstUserGroupRelationController.cs
- Added using statements: `GrpcServer.Infrastructure.Enum` and `Microsoft.Extensions.DependencyInjection`
- Updated constructor parameters with `[FromKeyedServices(AppCode.TST)]` for:
  - `IUserGroupRelationService<TstUser, TstGroup>`
  - `TstMapper`

### 3. Services
All three service implementations updated with `[FromKeyedServices(AppCode.TST)]` attribute:

#### TstUserService.cs
- Added using statements: `GrpcServer.Infrastructure.Enum` and `Microsoft.Extensions.DependencyInjection`
- Updated constructor parameters with `[FromKeyedServices(AppCode.TST)]` for:
  - `IUserRepository<TstUser>`
  - `IValidator<TstUser>`

#### TstGroupService.cs
- Added using statements: `GrpcServer.Infrastructure.Enum` and `Microsoft.Extensions.DependencyInjection`
- Updated constructor parameters with `[FromKeyedServices(AppCode.TST)]` for:
  - `IGroupRepository<TstGroup>`
  - `IValidator<TstGroup>`

#### TstUserGroupRelationService.cs
- Added using statements: `GrpcServer.Infrastructure.Enum` and `Microsoft.Extensions.DependencyInjection`
- Updated constructor parameters with `[FromKeyedServices(AppCode.TST)]` for:
  - `IUserGroupRelationRepository`
  - `IUserRepository<TstUser>`
  - `IGroupRepository<TstGroup>`

### 4. Data Seeder

#### TstDataSeeder.cs
- Added using statements: `GrpcServer.Infrastructure.Enum` and `Microsoft.Extensions.DependencyInjection`
- Updated constructor parameters with `[FromKeyedServices(AppCode.TST)]` for:
  - `IUserRepository<TstUser>`
  - `IGroupRepository<TstGroup>`
  - `IUserGroupRelationRepository`

## Key Benefits

1. **Multi-tenancy Support**: The keyed services pattern allows multiple implementations of the same service interface to coexist, keyed by `AppCode` enum (INM, ABC, TST).

2. **Type Safety**: Using the `AppCode` enum as the key ensures compile-time safety and prevents magic strings.

3. **Dependency Injection**: The `[FromKeyedServices]` attribute in constructors ensures the correct service implementation is injected based on the key.

4. **Scalability**: Easy to add new application codes (e.g., INM, ABC) with their own service implementations without conflicts.

## Testing

All test files in `GrpcServer.Tests` remain unchanged as they use direct instantiation rather than dependency injection, which is appropriate for unit testing.

## Future Additions

When adding new application codes (e.g., INM, ABC):
1. Create service implementations for the new code
2. Register them in `Program.cs` with the appropriate `AppCode` key
3. Create controllers that inject services using `[FromKeyedServices(AppCode.XXX)]`
4. All services at each layer (repositories, services, validators, mappers) should use the same key

