using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Infrastructure.Identity.Managers;

public class VolunteerManager : IVolunteerManager
{
    private readonly AccountDBContext _accountContext;

    public VolunteerManager(AccountDBContext accountContext)
    {
        _accountContext = accountContext;
    }

    public async Task<Result<VolunteerAccount, ErrorList>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountContext.VolunteerAccounts.FirstOrDefaultAsync(d => d.UserId == userId);

        if (account is null)
            return ErrorHelper.General.NotFound(userId).ToErrorList();

        return account;
    }

    public async Task<UnitResult<ErrorList>> UpdateRequisites(
        VolunteerAccount volunteerAccount,
        List<Requisites> requisites,
        CancellationToken cancellationToken = default)
    {
        volunteerAccount.Requisites = requisites;

        await _accountContext.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<ErrorList>();
    }
}
