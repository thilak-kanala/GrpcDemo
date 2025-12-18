# Mapper and Validator Dependency Injection Implementation

## Overview
This document describes the implementation of Dependency Injection for Mappers and Validators across all controllers in the GrpcServer application.

## Changes Made

### 1. Program.cs - DI Registration
Registered all mappers and validators as keyed services in the DI container:

#### ABC Mappers
- `IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto>` → `AbcMapper`
- `IMapper<AbcGroup, AbcGroupRequestDto, AbcGroupResponseDto>` → `AbcMapper`

#### ABC Validators
- `IUserValidator` → `AbcUserValidator`
- `IGroupValidator` → `AbcGroupValidator`

#### INM Mappers
- `IMapper<InmUser, InmUserRequestDto, InmUserResponseDto>` → `InmMapper`
- `IMapper<InmGroup, InmGroupRequestDto, InmGroupResponseDto>` → `InmMapper`

#### INM Validators
- `IUserValidator` → `InmUserValidator`
- `IGroupValidator` → `InmGroupValidator`

All registrations use `AddKeyedSingleton` with `AppCode.ABC` or `AppCode.INM` keys.

### 2. Controller Updates

#### Updated Controllers:
1. **AbcUsersController** - Now injects mapper and validator
2. **AbcGroupsController** - Now injects mapper and validator
3. **AbcUserGroupRelationsController** - Now injects user and group mappers
4. **InmUsersController** - Now injects mapper and validator
5. **InmGroupsController** - Now injects mapper and validator
6. **InmUserGroupRelationsController** - Now injects user and group mappers

#### Changes in Each Controller:

**Before:**
```csharp
public class AbcUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AbcUsersController([FromKeyedServices(AppCode.ABC)] IUserService userService)
    {
        _userService = userService;
    }
    
    // Using static mapper methods
    var users = await _userService.GetAllAsync();
    return Ok(users.Cast<AbcUser>().Select(AbcUserMapper.ToResponseDto));
}
```

**After:**
```csharp
public class AbcUsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> _mapper;
    private readonly IUserValidator _validator;

    public AbcUsersController(
        [FromKeyedServices(AppCode.ABC)] IUserService userService,
        [FromKeyedServices(AppCode.ABC)] IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> mapper,
        [FromKeyedServices(AppCode.ABC)] IUserValidator validator)
    {
        _userService = userService;
        _mapper = mapper;
        _validator = validator;
    }
    
    // Using injected mapper instances
    var users = await _userService.GetAllAsync();
    return Ok(users.Cast<AbcUser>().Select(_mapper.ToResponseDto));
}
```

### 3. Validation Integration

Added validation logic to all POST, PUT, and PATCH endpoints:

```csharp
[HttpPost]
public async Task<ActionResult<AbcUserResponseDto>> CreateUser([FromBody] AbcUserRequestDto dto)
{
    var user = _mapper.FromRequestDto(dto);
    
    if (!_validator.IsValid(user))
        return BadRequest(new { message = "Invalid user data" });
    
    await _userService.AddAsync(user);
    return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, _mapper.ToResponseDto(user));
}
```

## Benefits

### 1. **Testability**
- Controllers can now be unit tested with mock mappers and validators
- No dependency on static methods
- Easier to inject test doubles

### 2. **Flexibility**
- Mappers and validators can be swapped at runtime
- Different implementations can be used per environment
- Supports decorator pattern for additional functionality

### 3. **Consistency**
- All controllers follow the same DI pattern
- Centralized configuration in Program.cs
- Easier to maintain and extend

### 4. **Validation**
- Proper input validation on all create/update operations
- Returns 400 Bad Request with descriptive messages
- Prevents invalid data from being persisted

### 5. **SOLID Principles**
- **Single Responsibility**: Controllers delegate mapping and validation
- **Dependency Inversion**: Controllers depend on abstractions (interfaces)
- **Open/Closed**: Easy to extend with new mappers/validators without modifying controllers

## API Response Changes

### New Response Codes
- **400 Bad Request**: Returned when validation fails on POST/PUT/PATCH operations
  - Example: `{ "message": "Invalid user data" }`

### Updated ProducesResponseType Attributes
All create and update endpoints now include:
```csharp
[ProducesResponseType(StatusCodes.Status400BadRequest)]
```

## Validation Rules

### User Validators (ABC & INM)
- `Id` must be >= 0
- `UserName` must not be null/whitespace
- `Email` must not be null/whitespace and must contain '@'
- `SourceSystem` (ABC) or `InmHost` (INM) must not be null/whitespace

### Group Validators (ABC & INM)
- `Id` must be >= 0
- `DisplayName` must not be null/whitespace
- `TenantId` (ABC) or `InmHost` (INM) must not be null/whitespace

## Testing Recommendations

1. **Unit Tests**: Test controllers with mocked dependencies
2. **Integration Tests**: Verify DI container correctly resolves keyed services
3. **Validation Tests**: Test all validation scenarios (valid/invalid inputs)
4. **E2E Tests**: Verify end-to-end workflows with actual HTTP requests

## Future Enhancements

1. **FluentValidation**: Consider using FluentValidation library for more expressive rules
2. **Custom Validation Attributes**: Create reusable data annotation attributes
3. **Detailed Error Messages**: Return specific field-level validation errors
4. **Logging**: Add logging for validation failures
5. **Metrics**: Track validation failure rates

