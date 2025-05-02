using CSharpFunctionalExtensions;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Interfaces;

public interface IParticipantManager
{
    public Task<UnitResult<ErrorList>> CreateAsync(
        ParticipantAccount participant,
        CancellationToken cancellationToken = default);
}
