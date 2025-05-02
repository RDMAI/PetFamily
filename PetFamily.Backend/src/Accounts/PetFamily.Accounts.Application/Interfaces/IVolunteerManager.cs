using CSharpFunctionalExtensions;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Application.Interfaces;

public interface IVolunteerManager
{
    public Task<Result<VolunteerAccount, ErrorList>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    public Task<UnitResult<ErrorList>> UpdateRequisites(
        VolunteerAccount volunteerAccount,
        List<Requisites> requisites,
        CancellationToken cancellationToken = default);
}
