using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Infrastructure.Extensions;
using PetFamily.Accounts.Application.Exstensions;
using PetFamily.Accounts.Contracts;

namespace PetFamily.Accounts.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddAccounts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAccountsContract, AccountsContract>();

        services.AddInfrastructure(configuration)
            .AddApplication();

        return services;
    }
}
