# TST API Endpoints Quick Reference

## Base URL
`http://localhost:5185/api/v1/tst`

## User Endpoints

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| GET | `/users` | Get all users | 200, 500 |
| GET | `/users/{id}` | Get user by ID | 200, 404, 500 |
| POST | `/users` | Create new user | 201, 400, 500 |
| PUT | `/users/{id}` | Update user | 204, 400, 404, 500 |
| DELETE | `/users/{id}` | Delete user | 204, 404, 500 |

## Group Endpoints

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| GET | `/groups` | Get all groups | 200, 500 |
| GET | `/groups/{id}` | Get group by ID | 200, 404, 500 |
| POST | `/groups` | Create new group | 201, 400, 500 |
| PUT | `/groups/{id}` | Update group | 204, 400, 404, 500 |
| DELETE | `/groups/{id}` | Delete group | 204, 404, 500 |

## Relationship Endpoints

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| GET | `/users/{userId}/groups` | Get user's groups | 200, 404, 500 |
| POST | `/users/{userId}/groups` | Add user to groups | 204, 400, 404, 500 |
| DELETE | `/users/{userId}/groups/{groupId}` | Remove user from group | 204, 404, 500 |
| GET | `/groups/{groupId}/users` | Get group's users | 200, 404, 500 |
| POST | `/groups/{groupId}/users` | Add users to group | 204, 400, 404, 500 |

## Sample Requests

### Create User
```bash
curl -X POST http://localhost:5185/api/v1/tst/users \
  -H "Content-Type: application/json" \
  -d '{
    "id": "user1",
    "userName": "john.doe",
    "email": "john@example.com",
    "tstUserExtension1": "ext1",
    "tstUserExtension2": "ext2"
  }'
```

### Create Group
```bash
curl -X POST http://localhost:5185/api/v1/tst/groups \
  -H "Content-Type: application/json" \
  -d '{
    "id": "group1",
    "displayName": "Engineering",
    "tstGroupExtension1": "dept-eng",
    "tstGroupExtension2": "floor-3"
  }'
```

### Add User to Groups
```bash
curl -X POST http://localhost:5185/api/v1/tst/users/user1/groups \
  -H "Content-Type: application/json" \
  -d '{
    "groupIds": ["group1", "group2"]
  }'
```

### Add Users to Group
```bash
curl -X POST http://localhost:5185/api/v1/tst/groups/group1/users \
  -H "Content-Type: application/json" \
  -d '{
    "userIds": ["user1", "user2", "user3"]
  }'
```

### Get User's Groups
```bash
curl http://localhost:5185/api/v1/tst/users/user1/groups
```

### Get Group's Users
```bash
curl http://localhost:5185/api/v1/tst/groups/group1/users
```

## Scalar API Documentation
Access modern, interactive API documentation at: `http://localhost:5185/scalar/v1`

The API documentation is powered by **Scalar**, providing:
- Modern, clean interface
- Interactive API testing
- Code examples in multiple languages (C#, JavaScript, Python, etc.)
- Real-time request/response visualization

