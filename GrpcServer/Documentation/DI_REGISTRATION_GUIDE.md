# Dependency Injection Registration Guide

## Overview
This document describes the keyed dependency injection registrations for mappers and validators in the GrpcServer project.

## Registered Components

### ABC (External System) - AppCode.ABC

#### Mappers (Singleton)
- `AbcMapper` - Combined mapper class implementing:
  - `IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto>`
  - `IMapper<AbcGroup, AbcGroupRequestDto, AbcGroupResponseDto>`

The same `AbcMapper` instance is registered for both user and group mapping interfaces.

#### Validators (Singleton)
- `IUserValidator` → `AbcUserValidator`
- `IGroupValidator` → `AbcGroupValidator`

### INM (In-Memory) - AppCode.INM

#### Mappers (Singleton)
- `InmMapper` - Combined mapper class implementing:
  - `IMapper<InmUser, InmUserRequestDto, InmUserResponseDto>`
  - `IMapper<InmGroup, InmGroupRequestDto, InmGroupResponseDto>`

The same `InmMapper` instance is registered for both user and group mapping interfaces.

#### Validators (Singleton)
- `IUserValidator` → `InmUserValidator`
- `IGroupValidator` → `InmGroupValidator`

## Usage Examples

### Injecting Keyed Mappers in Controllers

```csharp
using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Mappers.Generic;
using GrpcServer.Infrastructure.Models.ABC;
using GrpcServer.Infrastructure.Models.ABC.DTO;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/v1/abc/users")]
public class AbcUsersController : ControllerBase
{
    private readonly IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> _mapper;

    public AbcUsersController(
        [FromKeyedServices(AppCode.ABC)] IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> mapper)
    {
        _mapper = mapper;
    }

    [HttpPost]
    public ActionResult<AbcUserResponseDto> CreateUser([FromBody] AbcUserRequestDto dto)
    {
        var user = _mapper.FromRequestDto(dto);
        // ... save user logic
        return Ok(_mapper.ToResponseDto(user));
    }
}
```

### Injecting Keyed Validators in Services

```csharp
using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Validators.Generic;

public class AbcUserService
{
    private readonly IUserValidator _validator;

    public AbcUserService(
        [FromKeyedServices(AppCode.ABC)] IUserValidator validator)
    {
        _validator = validator;
    }

    public async Task<bool> CreateUser(IBaseUser user)
    {
        if (!_validator.IsValid(user))
        {
            throw new ValidationException("Invalid user data");
        }
        // ... create user logic
    }
}
```

### Injecting Multiple Keyed Services

```csharp
public class AbcUsersController : ControllerBase
{
    private readonly IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> _mapper;
    private readonly IUserValidator _validator;
    private readonly IUserService _userService;

    public AbcUsersController(
        [FromKeyedServices(AppCode.ABC)] IMapper<AbcUser, AbcUserRequestDto, AbcUserResponseDto> mapper,
        [FromKeyedServices(AppCode.ABC)] IUserValidator validator,
        [FromKeyedServices(AppCode.ABC)] IUserService userService)
    {
        _mapper = mapper;
        _validator = validator;
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<AbcUserResponseDto>> CreateUser([FromBody] AbcUserRequestDto dto)
    {
        var user = _mapper.FromRequestDto(dto);
        
        if (!_validator.IsValid(user))
        {
            return BadRequest("Invalid user data");
        }

        await _userService.AddAsync(user);
        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, _mapper.ToResponseDto(user));
    }
}
```

## Benefits of Keyed Services

1. **Type Safety**: Each application code (ABC, INM) gets its own implementation.
2. **Clear Separation**: Different systems can have different validation and mapping logic.
3. **Easy Testing**: Mock specific implementations by key.
4. **Scalability**: Add new systems (e.g., AppCode.DEM) without conflicts.

## Validator Logic

### AbcUserValidator
- Id must be >= 0
- UserName must not be null or whitespace
- Email must not be null/whitespace and must contain '@'
- SourceSystem must not be null or whitespace

### AbcGroupValidator
- Id must be >= 0
- DisplayName must not be null or whitespace
- TenantId must not be null or whitespace

### InmUserValidator
- Id must be >= 0
- UserName must not be null or whitespace
- Email must not be null/whitespace and must contain '@'
- InmHost must not be null or whitespace

### InmGroupValidator
- Id must be >= 0
- DisplayName must not be null or whitespace
- InmHost must not be null or whitespace

## Generic Interfaces

### IMapper<TEntity, TRequestDto, TResponseDto>
```csharp
public interface IMapper<TEntity, TRequestDto, TResponseDto>
{
    TResponseDto ToResponseDto(TEntity entity);
    TEntity FromRequestDto(TRequestDto dto);
    void ApplyPatch(TEntity entity, TRequestDto dto);
}
```

### IValidator<T>
```csharp
public interface IValidator<T>
{
    bool IsValid(T entity);
}
```

### IUserValidator and IGroupValidator
```csharp
public interface IUserValidator : IValidator<IBaseUser> { }
public interface IGroupValidator : IValidator<IBaseGroup> { }
```

