using FluentValidation;
using PetFamily.Accounts.Application.DTOs;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Application.Commands.UpdateUserSocialNetworks;

public class UpdateUserSocialNetworksCommandValidator : AbstractValidator<UpdateUserSocialNetworksCommand>
{
    public UpdateUserSocialNetworksCommandValidator()
    {
        RuleFor(c => c.SocialNetworks)
            .MustBeNotNull();

        RuleForEach(c => c.SocialNetworks).MustBeValueObject((SocialNetworkDTO dto) =>
            SocialNetwork.Create(dto.Name, dto.Link));

        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("UserId"));
    }
}
