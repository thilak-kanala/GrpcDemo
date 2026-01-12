# Test Data Seeder Implementation

## Overview
Added automatic test data generation during application startup to populate the system with sample users, groups, and relationships.

## Implementation

### New File: `TstDataSeeder.cs`
Location: `GrpcServer/Infrastructure/Services/TST/TstDataSeeder.cs`

**Purpose:** Seeds initial test data for development and testing purposes.

**Features:**
- Checks if data already exists to avoid duplicate seeding
- Creates 7 test users
- Creates 6 test groups
- Establishes 14 user-group relationships
- Logs successful seeding to console

### Test Data Created

#### Users (7 total)
1. **john.doe** (Engineering, Senior Developer) - ID: user001
   - **Laptop OS:** macOS
   - **Phone OS:** iOS
2. **jane.smith** (Product, Product Manager) - ID: user002
   - **Laptop OS:** Windows
   - **Phone OS:** Android
3. **bob.johnson** (Engineering, Tech Lead) - ID: user003
   - **Laptop OS:** Linux
   - **Phone OS:** Android
4. **alice.williams** (Design, UX Designer) - ID: user004
   - **Laptop OS:** macOS
   - **Phone OS:** iOS
5. **charlie.brown** (Engineering, Junior Developer) - ID: user005
   - **Laptop OS:** Windows
   - **Phone OS:** Android
6. **diana.prince** (Marketing, Marketing Specialist) - ID: user006
   - **Laptop OS:** Linux
   - **Phone OS:** iOS
7. **evan.clark** (Operations, DevOps Engineer) - ID: user007
   - **Laptop OS:** macOS
   - **Phone OS:** iOS

#### Groups (6 total)
1. **Engineering Team** - ID: group001 (**Floor 3**, Building A)
2. **Product Management** - ID: group002 (**Floor 2**, Building B)
3. **Design Team** - ID: group003 (**Floor 2**, Building C)
4. **Leadership** - ID: group004 (**Floor 5**, Building A)
5. **DevOps Team** - ID: group005 (**Floor 3**, Building D)
6. **Marketing Team** - ID: group006 (**Floor 1**, Building B)

#### Relationships (14 total)
- **john.doe** → Engineering Team, Leadership
- **jane.smith** → Product Management, Leadership
- **bob.johnson** → Engineering Team, Leadership
- **alice.williams** → Design Team
- **charlie.brown** → Engineering Team
- **diana.prince** → Marketing Team
- **evan.clark** → Engineering Team, DevOps Team

### Program.cs Updates

**Service Registration:**
```csharp
// Register TST Data Seeder
builder.Services.AddScoped<TstDataSeeder>();
```

**Startup Seeding:**
```csharp
// Seed test data on startup
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<TstDataSeeder>();
    await seeder.SeedDataAsync();
}
```

## Benefits

✅ **Ready-to-Test API** - Data available immediately on startup
✅ **Realistic Scenarios** - Multiple users with overlapping group memberships
✅ **Hierarchical Structure** - Leadership group demonstrates cross-department membership
✅ **Idempotent** - Safe to restart application without duplicate data
✅ **Console Feedback** - Confirms successful seeding with count of records

## Usage

Simply run the application:
```bash
cd GrpcServer
dotnet run
```

On startup, you'll see:
```
✅ Seeded 7 users, 6 groups, and 14 relationships
```

Then access Swagger UI at `http://localhost:5185/swagger` to explore the API with pre-populated data.

## Example API Calls

**Get all users:**
```bash
curl http://localhost:5185/api/v1/tst/users
```

**Get Engineering Team members:**
```bash
curl http://localhost:5185/api/v1/tst/groups/group001/users
```

**Get john.doe's groups:**
```bash
curl http://localhost:5185/api/v1/tst/users/user001/groups
```

## Testing Tip

The seeder only runs once per application lifecycle. If you want to reset the data:
1. Restart the application (in-memory repositories clear on restart)
2. Data will be re-seeded automatically

---

**Created:** January 11, 2026
**Purpose:** Development and Testing
**Records:** 7 users, 6 groups, 14 relationships
