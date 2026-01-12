# TST Controller Unit Tests

This directory contains comprehensive unit tests for all TST controllers, ensuring complete code coverage and scenario validation.

## Test Files

### 1. TstUserControllerTests.cs
Comprehensive tests for the `TstUserController` which manages CRUD operations for TST Users.

**Test Coverage:**
- **GetAllUsers (3 tests)**
  - ✅ Returns OK with list of users when users exist
  - ✅ Returns OK with empty list when no users exist
  - ✅ Returns 500 Internal Server Error when service throws exception

- **GetUserById (3 tests)**
  - ✅ Returns OK with user when user exists
  - ✅ Returns 404 Not Found when user doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception

- **CreateUser (3 tests)**
  - ✅ Returns 201 Created with location header for valid user
  - ✅ Returns 400 Bad Request for invalid user data (validation failure)
  - ✅ Returns 500 Internal Server Error when service throws exception

- **UpdateUser (6 tests)**
  - ✅ Returns 204 No Content when updating existing user with valid data
  - ✅ Returns 400 Bad Request when URL ID doesn't match request body ID
  - ✅ Returns 404 Not Found when user doesn't exist
  - ✅ Returns 400 Bad Request when validation fails
  - ✅ Returns 500 Internal Server Error when service throws exception

- **DeleteUser (3 tests)**
  - ✅ Returns 204 No Content when deleting existing user
  - ✅ Returns 404 Not Found when user doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception

**Total: 18 tests**

---

### 2. TstGroupControllerTests.cs
Comprehensive tests for the `TstGroupController` which manages CRUD operations for TST Groups.

**Test Coverage:**
- **GetAllGroups (3 tests)**
  - ✅ Returns OK with list of groups when groups exist
  - ✅ Returns OK with empty list when no groups exist
  - ✅ Returns 500 Internal Server Error when service throws exception

- **GetGroupById (3 tests)**
  - ✅ Returns OK with group when group exists
  - ✅ Returns 404 Not Found when group doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception

- **CreateGroup (3 tests)**
  - ✅ Returns 201 Created with location header for valid group
  - ✅ Returns 400 Bad Request for invalid group data (validation failure)
  - ✅ Returns 500 Internal Server Error when service throws exception

- **UpdateGroup (6 tests)**
  - ✅ Returns 204 No Content when updating existing group with valid data
  - ✅ Returns 400 Bad Request when URL ID doesn't match request body ID
  - ✅ Returns 404 Not Found when group doesn't exist
  - ✅ Returns 400 Bad Request when validation fails
  - ✅ Returns 500 Internal Server Error when service throws exception

- **DeleteGroup (3 tests)**
  - ✅ Returns 204 No Content when deleting existing group
  - ✅ Returns 404 Not Found when group doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception

**Total: 18 tests**

---

### 3. TstUserGroupRelationControllerTests.cs
Comprehensive tests for the `TstUserGroupRelationController` which manages many-to-many relationships between users and groups.

**Test Coverage:**
- **GetUserGroups (4 tests)**
  - ✅ Returns OK with list of groups when user exists and has groups
  - ✅ Returns OK with empty list when user exists but has no groups
  - ✅ Returns 404 Not Found when user doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception

- **AddUserToGroups (6 tests)**
  - ✅ Returns 204 No Content when adding user to multiple groups successfully
  - ✅ Returns 400 Bad Request when group IDs list is empty
  - ✅ Returns 404 Not Found when user doesn't exist
  - ✅ Returns 404 Not Found when one of the groups doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception
  - ✅ Returns 204 No Content when adding user to single group

- **RemoveUserFromGroup (4 tests)**
  - ✅ Returns 204 No Content when removing user from group successfully
  - ✅ Returns 404 Not Found when user doesn't exist
  - ✅ Returns 404 Not Found when group doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception
  - ✅ Handles idempotent operations correctly

- **GetGroupUsers (4 tests)**
  - ✅ Returns OK with list of users when group exists and has users
  - ✅ Returns OK with empty list when group exists but has no users
  - ✅ Returns 404 Not Found when group doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception

- **AddUsersToGroup (6 tests)**
  - ✅ Returns 204 No Content when adding multiple users to group successfully
  - ✅ Returns 400 Bad Request when user IDs list is empty
  - ✅ Returns 404 Not Found when group doesn't exist
  - ✅ Returns 404 Not Found when one of the users doesn't exist
  - ✅ Returns 500 Internal Server Error when service throws exception
  - ✅ Returns 204 No Content when adding single user to group

- **Edge Cases (3 tests)**
  - ✅ Handles special characters in user IDs correctly
  - ✅ Handles special characters in group IDs correctly
  - ✅ Handles idempotent remove operations

**Total: 27 tests**

---

## Overall Summary

**Total Test Count: 63 tests**

### Coverage Statistics
- All controller endpoints are tested ✅
- All HTTP status codes are validated ✅
- All success paths are tested ✅
- All error paths are tested ✅
- Edge cases are covered ✅

### Test Scenarios Covered

#### ✅ Success Scenarios
- Successful CRUD operations (Create, Read, Update, Delete)
- Successful relationship management (Add/Remove users to/from groups)
- Empty result sets
- Single and bulk operations

#### ✅ Error Scenarios
- 400 Bad Request (validation failures, ID mismatches, empty lists)
- 404 Not Found (non-existent entities)
- 500 Internal Server Error (service exceptions)

#### ✅ Edge Cases
- Empty collections
- Single-item collections
- Special characters in IDs
- Idempotent operations
- Multiple enumeration prevention

### Testing Framework & Tools
- **Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: xUnit Assertions
- **Test Pattern**: AAA (Arrange-Act-Assert)

### Running the Tests

Run all controller tests:
```bash
dotnet test --filter "FullyQualifiedName~Controllers.TST"
```

Run specific controller tests:
```bash
# User Controller tests
dotnet test --filter "FullyQualifiedName~TstUserControllerTests"

# Group Controller tests
dotnet test --filter "FullyQualifiedName~TstGroupControllerTests"

# Relation Controller tests
dotnet test --filter "FullyQualifiedName~TstUserGroupRelationControllerTests"
```

Run with code coverage:
```bash
dotnet test --collect:"XPlat Code Coverage" --filter "FullyQualifiedName~Controllers.TST"
```

### Key Testing Principles Applied

1. **Isolation**: Each test uses mocked dependencies to ensure true unit testing
2. **Clarity**: Test names clearly describe what is being tested
3. **Completeness**: All code paths and scenarios are covered
4. **Independence**: Tests can run in any order without dependencies
5. **Verification**: All service calls are verified using Moq's Verify method
6. **Maintainability**: Tests follow consistent patterns and naming conventions

### Test Organization

Tests are organized into regions for easy navigation:
- GetAllUsers/GetAllGroups Tests
- GetUserById/GetGroupById Tests
- CreateUser/CreateGroup Tests
- UpdateUser/UpdateGroup Tests
- DeleteUser/DeleteGroup Tests
- Relationship Management Tests (for UserGroupRelation)
- Edge Cases and Additional Scenarios

Each test follows the AAA pattern:
- **Arrange**: Set up test data and mock behaviors
- **Act**: Execute the controller action
- **Assert**: Verify the results and service interactions

### Code Quality

✅ No compiler errors
✅ No runtime errors
✅ Warnings resolved (multiple enumeration, unused variables)
✅ Consistent naming conventions
✅ Comprehensive documentation
✅ Clean code principles applied

---

## Maintenance Notes

When adding new controller endpoints or modifying existing ones:
1. Add corresponding tests following the established patterns
2. Ensure all code paths are covered (success and error scenarios)
3. Verify mock interactions with `Verify()` calls
4. Test edge cases and boundary conditions
5. Update this README with new test counts and coverage information

## Related Documentation

- [API Endpoints Documentation](../../../GrpcServer/Infrastructure/Controllers/TST/API_ENDPOINTS.md)
- [Controller Implementation](../../../GrpcServer/Infrastructure/Controllers/TST/)
- [Service Tests](../../Services/TST/)
- [Repository Tests](../../Repositories/TST/)

