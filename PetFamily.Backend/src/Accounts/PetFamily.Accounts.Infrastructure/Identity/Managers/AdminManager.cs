using CSharpFunctionalExtensions;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Infrastructure.Identity.Managers;

public class AdminManager : IAdminManager
{
    private readonly AccountDBContext _accountContext;

    public AdminManager(AccountDBContext accountContext)
    {
        _accountContext = accountContext;
    }

    public async Task<UnitResult<ErrorList>> CreateAsync(
        AdminAccount adminAccount,
        CancellationToken cancellationToken = default)
    {
        _accountContext.AdminAccounts.Add(adminAccount);

        await _accountContext.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<ErrorList>();
    }
}
