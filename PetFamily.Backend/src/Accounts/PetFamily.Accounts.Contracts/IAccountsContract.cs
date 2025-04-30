using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Contracts;

public interface IAccountsContract
{
    public Task<UnitResult<ErrorList>> CheckPermissionByUserIdAsync(
        Guid userId,
        string permissionCode,
        CancellationToken cancellationToken = default);
}
