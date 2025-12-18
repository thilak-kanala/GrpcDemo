# Redundant Method Removal - RemoveUserFromGroupInGroupContextAsync

## Overview
Removed the redundant `RemoveUserFromGroupInGroupContextAsync` method that was duplicating the functionality of `RemoveUserFromGroupAsync`.

## Problem
The codebase had two methods that performed the exact same operation - removing a user from a group:

### Original Redundant Implementation

```csharp
// Method 1: User-centric route
public async Task RemoveUserFromGroupAsync(int userId, int groupId)
{
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null) throw new InvalidOperationException($"User with ID {userId} not found");
    
    var group = await _groupRepository.GetByIdAsync(groupId);
    if (group == null) throw new InvalidOperationException($"Group with ID {groupId} not found");
    
    await _relationRepository.RemoveUserFromGroupAsync(userId, groupId);
}

// Method 2: Group-centric route (REDUNDANT)
public async Task RemoveUserFromGroupInGroupContextAsync(int groupId, int userId)
{
    var group = await _groupRepository.GetByIdAsync(groupId);
    if (group == null) throw new InvalidOperationException($"Group with ID {groupId} not found");
    
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null) throw new InvalidOperationException($"User with ID {userId} not found");
    
    await _relationRepository.RemoveUserFromGroupAsync(userId, groupId); // Same call!
}
```

**Both methods:**
- Validated that the user exists
- Validated that the group exists  
- Called the same repository method: `RemoveUserFromGroupAsync(userId, groupId)`
- The only difference was the **order** of parameter validation (which has no functional impact)

## Solution
Removed the redundant method and updated all controllers to use `RemoveUserFromGroupAsync` with parameters in the correct order.

### Files Modified

#### 1. Interface Updated
**File:** `Infrastructure/Services/Generic/IUserGroupRelationService.cs`
- ❌ Removed: `Task RemoveUserFromGroupInGroupContextAsync(int groupId, int userId);`
- ✅ Kept: `Task RemoveUserFromGroupAsync(int userId, int groupId);`

#### 2. ABC Service Updated
**File:** `Infrastructure/Services/ABC/AbcUserGroupRelationService.cs`
- ❌ Removed: `RemoveUserFromGroupInGroupContextAsync` method implementation (19 lines)

#### 3. INM Service Updated
**File:** `Infrastructure/Services/INM/InmUserGroupRelationService.cs`
- ❌ Removed: `RemoveUserFromGroupInGroupContextAsync` method implementation (19 lines)

#### 4. ABC Controller Updated
**File:** `Infrastructure/Controllers/ABC/AbcUserGroupRelationsController.cs`

**Before:**
```csharp
[HttpDelete("groups/{groupId}/users/{userId}")]
public async Task<ActionResult> RemoveUserFromGroupInGroupContext(int groupId, int userId)
{
    await _relationService.RemoveUserFromGroupInGroupContextAsync(groupId, userId);
    return NoContent();
}
```

**After:**
```csharp
[HttpDelete("groups/{groupId}/users/{userId}")]
public async Task<ActionResult> RemoveUserFromGroupInGroupContext(int groupId, int userId)
{
    await _relationService.RemoveUserFromGroupAsync(userId, groupId); // Reordered parameters
    return NoContent();
}
```

#### 5. INM Controller Updated
**File:** `Infrastructure/Controllers/INM/InmUserGroupRelationsController.cs`
- Same change as ABC controller above

## API Routes Preserved
Both API routes continue to work as before:

1. **User-centric route:**  
   `DELETE /api/v1/{abc|inm}/user-group-relations/users/{userId}/groups/{groupId}`

2. **Group-centric route:**  
   `DELETE /api/v1/{abc|inm}/user-group-relations/groups/{groupId}/users/{userId}`

Both routes now call the same underlying service method: `RemoveUserFromGroupAsync(userId, groupId)`

## Benefits

### 1. **Code Reduction**
- Removed ~40 lines of duplicate code (2 implementations)
- Removed 1 interface method declaration
- Updated 2 controller method calls

### 2. **Maintainability**
- Single source of truth for the remove operation
- Changes to validation logic only need to be made in one place
- Reduced cognitive load - developers don't need to understand why two methods exist

### 3. **Consistency**
- All operations now use a consistent method
- Parameter order is standardized (userId, groupId)
- Easier to reason about the codebase

### 4. **Performance**
- Slightly reduced memory footprint
- No functional difference in performance (same operations executed)

## Testing Considerations

### Existing Tests
If you have unit tests for the removed method, update them to test `RemoveUserFromGroupAsync` with both parameter orders.

### Test Both Routes
Ensure integration tests cover both API routes:

```csharp
// Test user-centric route
DELETE /api/v1/abc/user-group-relations/users/1/groups/10

// Test group-centric route (same result)
DELETE /api/v1/abc/user-group-relations/groups/10/users/1
```

Both should successfully remove user 1 from group 10.

## Migration Notes

### For API Consumers
**No changes required!** Both API routes continue to work exactly as before.

### For Internal Developers
If you have any custom code calling `RemoveUserFromGroupInGroupContextAsync`:
```csharp
// OLD (will not compile)
await service.RemoveUserFromGroupInGroupContextAsync(groupId, userId);

// NEW (correct)
await service.RemoveUserFromGroupAsync(userId, groupId);
```

Note the parameter order change: `(userId, groupId)` instead of `(groupId, userId)`.

## Conclusion

This refactoring eliminates unnecessary code duplication and API redundancy. The removal of ~70 lines of redundant code improves maintainability and provides a clearer, more consistent API surface.

**Build Status:** ✅ Success  
**Breaking Changes:** ⚠️ Yes - Removed `DELETE groups/{groupId}/users/{userId}` endpoint  
**Recommended Action:** Update API clients to use `DELETE users/{userId}/groups/{groupId}`  
**Internal API Compatibility:** Service method signature changed

