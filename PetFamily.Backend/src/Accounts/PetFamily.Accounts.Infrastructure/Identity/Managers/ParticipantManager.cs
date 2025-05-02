using CSharpFunctionalExtensions;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Infrastructure.Identity.Managers;

public class ParticipantManager : IParticipantManager
{
    private readonly AccountDBContext _accountContext;

    public ParticipantManager(AccountDBContext accountContext)
    {
        _accountContext = accountContext;
    }

    public async Task<UnitResult<ErrorList>> CreateAsync(
        ParticipantAccount participant,
        CancellationToken cancellationToken = default)
    {
        _accountContext.ParticipantAccounts.Add(participant);

        await _accountContext.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<ErrorList>();
    }
}
