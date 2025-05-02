using CSharpFunctionalExtensions;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Infrastructure.Identity.Managers;

public class UserInfoManager : IUserInfoManager
{
    private readonly AccountDBContext _accountContext;

    public UserInfoManager(
        AccountDBContext accountContext)
    {
        _accountContext = accountContext;
    }

    public async Task<UnitResult<ErrorList>> UpdateSocialNetworksAsync(
        User user,
        List<SocialNetwork> socialNetworks,
        CancellationToken cancellationToken = default)
    {
        user.SocialNetworks = socialNetworks;

        await _accountContext.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<ErrorList>();
    }

    public async Task<UnitResult<ErrorList>> UpdateFullNameAsync(
        User user,
        string firstName,
        string lastName,
        string fatherName,
        CancellationToken cancellationToken = default)
    {
        user.FirstName = firstName;
        user.LastName = lastName;
        user.FatherName = fatherName;

        await _accountContext.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<ErrorList>();
    }
}
