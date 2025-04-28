using Microsoft.AspNetCore.Mvc;
using PetFamily.Accounts.Infrastructure.Identity;
using PetFamily.Shared.Framework.Extensions;
using Serilog;
using Serilog.Events;

namespace PetFamily.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services)
    {
        // disables controller's filter, that validaties model state before entering controller;
        // instead, it will pass invalid model to controller and then tp application layer,
        // where we validate the model with fluent validation
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddPermissions();

        return services;
    }

    public static IServiceCollection AddAPILogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Seq(configuration.GetConnectionString("Seq")
                ?? throw new ArgumentNullException("Seq"))
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();

        // to watch serilogs inner errors
        Serilog.Debugging.SelfLog.Enable(Console.Error);

        services.AddSerilog();

        return services;
    }

    public static async Task ApplyModulesMigrations(this IHost host)
    {
        await PetsManagement.Infrastructure.Extensions.DependencyInjection.ApplyMigrations(host);
        await SpeciesManagement.Infrastructure.Extensions.DependencyInjection.ApplyMigrations(host);
        await Accounts.Infrastructure.Extensions.DependencyInjection.ApplyMigrations(host);
    }

    public static async Task SeedAccounts(this IHost host)
    {
        var seeder = host.Services.GetRequiredService<AccountSeeder>();

        await seeder.SeedAsync();
    }
}
