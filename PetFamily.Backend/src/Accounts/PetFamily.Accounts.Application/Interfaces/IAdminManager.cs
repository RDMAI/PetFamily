using CSharpFunctionalExtensions;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Interfaces;

public interface IAdminManager
{
    public Task<UnitResult<ErrorList>> CreateAsync(
        AdminAccount adminAccount,
        CancellationToken cancellationToken = default);
}
