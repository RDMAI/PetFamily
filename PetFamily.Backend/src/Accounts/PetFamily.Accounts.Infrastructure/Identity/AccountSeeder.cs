using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain;
using System.Text.Json;

namespace PetFamily.Accounts.Infrastructure.Identity;

public class AccountSeeder
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccountSeeder> _logger;

    public AccountSeeder(
        IServiceScopeFactory scopeFactory,
        ILogger<AccountSeeder> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Account seeding started");

        var json = await File.ReadAllTextAsync("etc/accounts.json");

        using var scope = _scopeFactory.CreateScope();

        var accountsContext = scope.ServiceProvider.GetRequiredService<AccountDBContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        var seedData = JsonSerializer.Deserialize<RolePermissionConfig>(json);
        if (seedData is null)
            throw new ApplicationException("Could not deserialize permissions and roles from Accounts.json");

        var permissionsFromConfig = seedData.Permissions.SelectMany(p => p.Value).ToList();

        // seed Permissions
        List<Permission> permissions = [];
        foreach (var permissionCode in permissionsFromConfig)
        {
            var existingPermission = await accountsContext.Permissions.FirstOrDefaultAsync(p => p.Code == permissionCode);

            if (existingPermission is not null)
                continue;

            permissions.Add(new Permission { Code = permissionCode });
        }
        accountsContext.Permissions.AddRange(permissions);

        // seed Roles with RolePermisssions
        foreach (var roleConfig in seedData.Roles)
        {
            var existingRole = await accountsContext.Roles.FirstOrDefaultAsync(p => p.Name == roleConfig.Key);

            if (existingRole is not null)
                continue;

            var role = new Role { Name = roleConfig.Key };
            await roleManager.CreateAsync(role);  // calls SaveChangesAsync each time

            var permissionsToRole = await accountsContext.Permissions.Where(p => roleConfig.Value.Contains(p.Code)).ToListAsync();
            foreach (var permission in permissionsToRole)
            {
                accountsContext.RolePermissions.Add(new RolePermission
                {
                    PermissionId = permission.Id,
                    RoleId = role.Id
                });
            }
        }

        await accountsContext.SaveChangesAsync();

        _logger.LogInformation("Account seeding finished");
    }

    class RolePermissionConfig
    {
        public Dictionary<string, string[]> Permissions { get; set; } = [];
        public Dictionary<string, string[]> Roles { get; set; } = [];
    }
}
