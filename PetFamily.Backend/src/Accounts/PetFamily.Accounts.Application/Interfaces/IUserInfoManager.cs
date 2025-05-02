using CSharpFunctionalExtensions;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Interfaces;

public interface IUserInfoManager
{
    public Task<UnitResult<ErrorList>> UpdateSocialNetworksAsync(
        User user,
        List<SocialNetwork> socialNetworks,
        CancellationToken cancellationToken = default);

    public Task<UnitResult<ErrorList>> UpdateFullNameAsync(
        User user,
        string firstName,
        string lastName,
        string fatherName,
        CancellationToken cancellationToken = default);
}
