# Quick Start Guide

## Run the API

```bash
cd GrpcServer
dotnet run
```

## Test with Swagger UI

1. Open browser to: **http://localhost:5001**
2. You'll see the Swagger UI with all 18 endpoints organized by category

## Example Workflow

### 1. Create a User
- Navigate to **POST /users**
- Click "Try it out"
- Enter:
```json
{
  "userName": "alice",
  "email": "alice@example.com"
}
```
- Click "Execute"
- Note the returned user ID (e.g., 1234)

### 2. Create a Group
- Navigate to **POST /groups**
- Click "Try it out"
- Enter:
```json
{
  "displayName": "Developers"
}
```
- Click "Execute"
- Note the returned group ID (e.g., 5678)

### 3. Add User to Group
- Navigate to **POST /users/{userId}/groups**
- Click "Try it out"
- Enter userId: `1234`
- Request body:
```json
{
  "groupIds": [5678]
}
```
- Click "Execute"

### 4. Get User's Groups
- Navigate to **GET /users/{userId}/groups**
- Click "Try it out"
- Enter userId: `1234`
- Click "Execute"
- You'll see the group in the response

### 5. Update User Email (Partial Update)
- Navigate to **PATCH /users/{userId}**
- Click "Try it out"
- Enter userId: `1234`
- Request body:
```json
{
  "email": "alice.updated@example.com"
}
```
- Click "Execute"

### 6. List All Users
- Navigate to **GET /users**
- Click "Try it out"
- Click "Execute"
- See all users

## API Categories in Swagger

### 👥 Users
- List, Create, Get, Update, Patch, Delete users

### 👪 Groups  
- List, Create, Get, Update, Patch, Delete groups

### 🔗 Relations
- Manage user-group associations (both directions)

## All Endpoints

```
Users:
  GET    /users
  POST   /users
  GET    /users/{userId}
  PUT    /users/{userId}
  PATCH  /users/{userId}
  DELETE /users/{userId}

Groups:
  GET    /groups
  POST   /groups
  GET    /groups/{groupId}
  PUT    /groups/{groupId}
  PATCH  /groups/{groupId}
  DELETE /groups/{groupId}

Relations:
  GET    /users/{userId}/groups
  POST   /users/{userId}/groups
  DELETE /users/{userId}/groups/{groupId}
  GET    /groups/{groupId}/users
  POST   /groups/{groupId}/users
  DELETE /groups/{groupId}/users/{userId}
```

## Testing with cURL

### Create User
```bash
curl -X POST http://localhost:5001/users \
  -H "Content-Type: application/json" \
  -d '{"userName": "bob", "email": "bob@example.com"}'
```

### Get All Users
```bash
curl http://localhost:5001/users
```

### Partial Update
```bash
curl -X PATCH http://localhost:5001/users/1234 \
  -H "Content-Type: application/json" \
  -d '{"userName": "bobby"}'
```

## Next Steps

1. ✅ **Done**: All API structure is complete
2. 🔄 **TODO**: Implement actual HTTP calls in repositories (MckUserRepository, MckGroupRepository)
3. 🔄 **TODO**: Add real backend storage for user-group relations
4. 🔄 **TODO**: Add authentication/authorization if needed
5. 🔄 **TODO**: Add logging and monitoring
6. 🔄 **TODO**: Add integration tests

## Files Created

```
Controllers/
  ✅ UsersController.cs
  ✅ GroupsController.cs
  ✅ UserGroupRelationsController.cs

DTOs/
  ✅ UserDtos.cs
  ✅ GroupDtos.cs
  ✅ RelationDtos.cs

Mappers/
  ✅ UserMapper.cs
  ✅ GroupMapper.cs

Validators/
  ✅ UserValidator.cs
  ✅ GroupValidator.cs

Services/
  ✅ IUserGroupRelationService.cs
  ✅ UserGroupRelationService.cs

Documentation/
  ✅ openapi.yaml
  ✅ API_README.md
  ✅ IMPLEMENTATION_SUMMARY.md
  ✅ QUICKSTART.md
```

Happy coding! 🚀

