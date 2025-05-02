using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Accounts.Infrastructure.Identity.Managers;
using PetFamily.Accounts.Infrastructure.Identity.Options;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

namespace PetFamily.Accounts.Infrastructure.Identity;

public class AccountSeeder
{
    private readonly IServiceScope _scope;
    private readonly AdminOptions _adminOptions;
    private readonly ILogger<AccountSeeder> _logger;
    private readonly AccountDBContext _accountsContext;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IAdminManager _adminManager;

    public AccountSeeder(
        IServiceScopeFactory scopeFactory,
        IOptions<AdminOptions> adminOptions,
        ILogger<AccountSeeder> logger)
    {
        _scope = scopeFactory.CreateScope();

        _accountsContext = _scope.ServiceProvider.GetRequiredService<AccountDBContext>();
        _roleManager = _scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        _adminManager = _scope.ServiceProvider.GetRequiredService<IAdminManager>();

        _adminOptions = adminOptions.Value;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Account seeding started");

        var json = await File.ReadAllTextAsync("etc/accounts.json");

        var seedData = JsonSerializer.Deserialize<RolePermissionConfig>(json);
        if (seedData is null)
            throw new ApplicationException("Could not deserialize permissions and roles from Accounts.json");

        var permissionsFromConfig = seedData.Permissions.SelectMany(p => p.Value).ToList();

        // seed Permissions
        List<Permission> permissions = [];
        foreach (var permissionCode in permissionsFromConfig)
        {
            var existingPermission = await _accountsContext.Permissions.FirstOrDefaultAsync(p => p.Code == permissionCode);

            if (existingPermission is not null)
                continue;

            permissions.Add(new Permission { Code = permissionCode });
            _logger.LogInformation("Permission added {permissionCode}", permissionCode);
        }
        _accountsContext.Permissions.AddRange(permissions);

        // seed Roles with RolePermisssions
        foreach (var roleConfig in seedData.Roles)
        {
            var existingRole = await _accountsContext.Roles.FirstOrDefaultAsync(p => p.Name == roleConfig.Key);

            if (existingRole is not null)
                continue;

            var role = new Role { Name = roleConfig.Key };
            await _roleManager.CreateAsync(role);  // calls SaveChangesAsync each time
            _logger.LogInformation("Role created {roleName}", roleConfig.Key);

            var permissionsToRole = await _accountsContext.Permissions.Where(p => roleConfig.Value.Contains(p.Code)).ToListAsync();
            foreach (var permission in permissionsToRole)
            {
                _accountsContext.RolePermissions.Add(new RolePermission
                {
                    PermissionId = permission.Id,
                    RoleId = role.Id
                });
            }
        }

        await _accountsContext.SaveChangesAsync();

        var adminResult = await SeedAdminAsync();
        if (adminResult.IsSuccess)
            _logger.LogInformation("User Admin created");

        _scope.Dispose();

        _logger.LogInformation("Account seeding finished");
    }

    private async Task<UnitResult<string>> SeedAdminAsync()
    {
        _accountsContext.Database.BeginTransaction();

        var adminUser = new User
        {
            UserName = _adminOptions.Username,
            Email = _adminOptions.Email,
        };

        // this checks if user already exist
        var userResult = await _userManager.CreateAsync(adminUser, _adminOptions.Password);
        if (userResult.Succeeded == false)
            return userResult.Errors.First().Description;

        var roleResult = await _userManager.AddToRoleAsync(adminUser, AdminAccount.ROLE_NAME);
        if (roleResult.Succeeded == false)
            return roleResult.Errors.First().Description;

        var adminAccountResult = await _adminManager.CreateAsync(
            adminAccount: new AdminAccount { UserId = adminUser.Id });

        if (adminAccountResult.IsFailure)
            return adminAccountResult.Error.First().Message;

        await _accountsContext.Database.CommitTransactionAsync();

        return UnitResult.Success<string>();
    }

    class RolePermissionConfig
    {
        public Dictionary<string, string[]> Permissions { get; set; } = [];
        public Dictionary<string, string[]> Roles { get; set; } = [];
    }
}
