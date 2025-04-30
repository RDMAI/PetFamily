using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Accounts.Infrastructure.Identity;
using System.Text;

namespace PetFamily.Accounts.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString(DBConstants.DATABASE);
        services.AddScoped(_ => new AccountDBContext(dbConnectionString));

        services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 10;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
            })
            .AddEntityFrameworkStores<AccountDBContext>();

        services.Configure<JWTOptions>(configuration.GetSection(JWTOptions.CONFIG_NAME));
        services.AddOptions<JWTOptions>();
        services.AddScoped<ITokenHandler, JWTHandler>();

        services
            .AddAuthentication(options =>
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

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jWTOptions.Issuer,
                    ValidateIssuer = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jWTOptions.Key)),
                    ValidateIssuerSigningKey = true,
                    ValidAudience = jWTOptions.Audience,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = false
                };
            });

        services.AddAuthorization();

        services.AddScoped<IPermissionManager, PermissionManager>();
        services.AddSingleton<AccountSeeder>();

        return services;
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AccountDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}
