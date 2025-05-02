using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Infrastructure.Identity;
using PetFamily.Tests.Shared;

namespace PetFamily.Accounts.Application.Tests;

public class AccountsBaseTests : BaseHandlerTests
{
    protected readonly AccountDBContext _accountContext;

    public AccountsBaseTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _accountContext = _scope.ServiceProvider.GetRequiredService<AccountDBContext>();
    }

    public override async Task DisposeAsync()
    {
        _accountContext.Dispose();
        await base.DisposeAsync();
    }
}
