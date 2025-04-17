using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateSocialNetworks;
public class UpdateSocialNetworksCommandValidator : AbstractValidator<UpdateSocialNetworksCommand>
{
    public UpdateSocialNetworksCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.SocialNetworksList)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("SocialNetworks"));
        RuleForEach(c => c.SocialNetworksList).MustBeValueObject(CreateNetworksFromDTO);
    }

    private Result<SocialNetwork, Error> CreateNetworksFromDTO(SocialNetworkDTO dto)
        => SocialNetwork.Create(dto.Name, dto.Link);
}
