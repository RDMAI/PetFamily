﻿using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateSocialNetworks;
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
