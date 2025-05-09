using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Accounts.Infrastructure.Identity;
using PetFamily.Accounts.Infrastructure.Identity.Managers;
using PetFamily.Accounts.Infrastructure.Identity.Options;
using PetFamily.Shared.Core;
using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.Accounts.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // database access
        var dbConnectionString = configuration.GetConnectionString(DBConstants.DATABASE);
        services.AddScoped(_ => new AccountDBContext(dbConnectionString));

        services.AddKeyedScoped<IUnitOfWork, AccountUnitOfWork>(
            serviceKey: DependencyHelper.DependencyKey.Accounts);

        services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 10;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
            })
            .AddEntityFrameworkStores<AccountDBContext>()
            .AddDefaultTokenProviders();

        services.Configure<JWTOptions>(configuration.GetSection(JWTOptions.CONFIG_NAME));
        services.AddOptions<JWTOptions>();
        services.AddScoped<ITokenManager, TokenManager>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultScheme =
                options.DefaultForbidScheme =
                options.DefaultChallengeScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jWTOptions = configuration.GetSection(JWTOptions.CONFIG_NAME).Get<JWTOptions>()
                ?? throw new ApplicationException("JWT is not configured");

                options.TokenValidationParameters = TokenManager.GetTokenValidationParameters(jWTOptions);
            });

        services.AddAuthorization();

        services.AddScoped<IPermissionManager, PermissionManager>();
        services.AddScoped<IAdminManager, AdminManager>();
        services.AddScoped<IVolunteerManager, VolunteerManager>();
        services.AddScoped<IParticipantManager, ParticipantManager>();
        services.AddScoped<IUserInfoManager, UserInfoManager>();

        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.ADMIN));
        services.AddOptions<AdminOptions>();

        services.AddTransient<AccountSeeder>();

        return services;
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AccountDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}
