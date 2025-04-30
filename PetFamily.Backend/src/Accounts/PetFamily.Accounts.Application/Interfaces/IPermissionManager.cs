using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Interfaces;

public interface IPermissionManager
{
    public Task<UnitResult<ErrorList>> CheckPermissionByUserIdAsync(
        Guid userId,
        string permissionCode,
        CancellationToken cancellationToken = default);
}
