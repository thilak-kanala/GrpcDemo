using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Enum;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServer.Infrastructure.Services.TST;

/// <summary>
/// Service for seeding initial test data for TST users, groups, and relationships.
/// </summary>
public class TstDataSeeder
{
    private readonly IUserRepository<TstUser> _userRepository;
    private readonly IGroupRepository<TstGroup> _groupRepository;
    private readonly IUserGroupRelationRepository _relationRepository;

    public TstDataSeeder(
        [FromKeyedServices(AppCode.TST)] IUserRepository<TstUser> userRepository,
        [FromKeyedServices(AppCode.TST)] IGroupRepository<TstGroup> groupRepository,
        [FromKeyedServices(AppCode.TST)] IUserGroupRelationRepository relationRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _relationRepository = relationRepository;
    }

    /// <summary>
    /// Seeds the database with test users, groups, and relationships.
    /// </summary>
    public async Task SeedDataAsync()
    {
        // Check if data already exists
        var existingUsers = await _userRepository.GetAllAsync();
        if (existingUsers.Any())
        {
            // Data already seeded, skip
            return;
        }

        // Create test users
        var users = new[]
        {
            new TstUser
            {
                Id = "user001",
                UserName = "john.doe",
                Email = "john.doe@example.com",
                TstUserExtension1 = "macOS",
                TstUserExtension2 = "iOS"
            },
            new TstUser
            {
                Id = "user002",
                UserName = "jane.smith",
                Email = "jane.smith@example.com",
                TstUserExtension1 = "Windows",
                TstUserExtension2 = "Android"
            },
            new TstUser
            {
                Id = "user003",
                UserName = "bob.johnson",
                Email = "bob.johnson@example.com",
                TstUserExtension1 = "Linux",
                TstUserExtension2 = "Android"
            },
            new TstUser
            {
                Id = "user004",
                UserName = "alice.williams",
                Email = "alice.williams@example.com",
                TstUserExtension1 = "macOS",
                TstUserExtension2 = "iOS"
            },
            new TstUser
            {
                Id = "user005",
                UserName = "charlie.brown",
                Email = "charlie.brown@example.com",
                TstUserExtension1 = "Windows",
                TstUserExtension2 = "Android"
            },
            new TstUser
            {
                Id = "user006",
                UserName = "diana.prince",
                Email = "diana.prince@example.com",
                TstUserExtension1 = "Linux",
                TstUserExtension2 = "iOS"
            },
            new TstUser
            {
                Id = "user007",
                UserName = "evan.clark",
                Email = "evan.clark@example.com",
                TstUserExtension1 = "macOS",
                TstUserExtension2 = "iOS"
            }
        };

        // Create test groups
        var groups = new[]
        {
            new TstGroup
            {
                Id = "group001",
                DisplayName = "Engineering Team",
                TstGroupExtension1 = "Floor 3",
                TstGroupExtension2 = "Building A"
            },
            new TstGroup
            {
                Id = "group002",
                DisplayName = "Product Management",
                TstGroupExtension1 = "Floor 2",
                TstGroupExtension2 = "Building B"
            },
            new TstGroup
            {
                Id = "group003",
                DisplayName = "Design Team",
                TstGroupExtension1 = "Floor 2",
                TstGroupExtension2 = "Building C"
            },
            new TstGroup
            {
                Id = "group004",
                DisplayName = "Leadership",
                TstGroupExtension1 = "Floor 5",
                TstGroupExtension2 = "Building A"
            },
            new TstGroup
            {
                Id = "group005",
                DisplayName = "DevOps Team",
                TstGroupExtension1 = "Floor 3",
                TstGroupExtension2 = "Building D"
            },
            new TstGroup
            {
                Id = "group006",
                DisplayName = "Marketing Team",
                TstGroupExtension1 = "Floor 1",
                TstGroupExtension2 = "Building B"
            }
        };

        // Add users to repository
        foreach (var user in users)
        {
            await _userRepository.AddAsync(user);
        }

        // Add groups to repository
        foreach (var group in groups)
        {
            await _groupRepository.AddAsync(group);
        }

        // Create user-group relationships
        var relationships = new[]
        {
            // John Doe - Engineering Team, Leadership
            ("user001", "group001"),
            ("user001", "group004"),
            
            // Jane Smith - Product Management, Leadership
            ("user002", "group002"),
            ("user002", "group004"),
            
            // Bob Johnson - Engineering Team, Leadership
            ("user003", "group001"),
            ("user003", "group004"),
            
            // Alice Williams - Design Team
            ("user004", "group003"),
            
            // Charlie Brown - Engineering Team
            ("user005", "group001"),
            
            // Diana Prince - Marketing Team
            ("user006", "group006"),
            
            // Evan Clark - Engineering Team, DevOps Team
            ("user007", "group001"),
            ("user007", "group005")
        };

        // Add relationships
        foreach (var (userId, groupId) in relationships)
        {
            await _relationRepository.AddUserToGroupAsync(userId, groupId);
        }

        Console.WriteLine($"✅ Seeded {users.Length} users, {groups.Length} groups, and {relationships.Length} relationships");
    }
}

