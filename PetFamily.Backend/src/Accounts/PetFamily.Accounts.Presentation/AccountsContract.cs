using CSharpFunctionalExtensions;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Contracts;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Presentation;

public class AccountsContract : IAccountsContract
{
    private readonly IPermissionManager _permissionManager;

    public AccountsContract(IPermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    public async Task<UnitResult<ErrorList>> CheckPermissionByUserIdAsync(
        Guid userId,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        return await _permissionManager.CheckPermissionByUserIdAsync(userId, permissionCode, cancellationToken);
    }
}
