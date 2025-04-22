using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Application.DataModels;
using PetFamily.Accounts.Application.Interfaces;
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

        var jWTOptions = configuration.Get<JWTOptions>()
                ?? throw new ApplicationException("JWT is not configured");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jWTOptions.Key)),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateLifetime = false
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AccountDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}
