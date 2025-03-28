﻿using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Primitives;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateSocialNetworks;
public class UpdateSocialNetworksHandler
    : ICommandHandler<VolunteerId, UpdateSocialNetworksCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly UpdateSocialNetworksCommandValidator _validator;
    private readonly ILogger<UpdateSocialNetworksHandler> _logger;

    public UpdateSocialNetworksHandler(
        IVolunteerAggregateRepository volunteerRepository,
        UpdateSocialNetworksCommandValidator validator,
        ILogger<UpdateSocialNetworksHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<VolunteerId, ErrorList>> HandleAsync(
        UpdateSocialNetworksCommand command,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validatorResult.IsValid)
        {
            var errors = from e in validatorResult.Errors
                         select Error.Deserialize(e.ErrorMessage);
            return new ErrorList(errors);
        }

        // check if volunteer exist
        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var entityResult = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Error;

        // create VOs
        List<SocialNetwork> socialNetworkBufferList = [];
        foreach (SocialNetworkDTO socialNetwork in command.SocialNetworksList)
        {
            socialNetworkBufferList.Add(SocialNetwork.Create(socialNetwork.Name,
                socialNetwork.Link).Value);
        }

        // update entity
        var entity = entityResult.Value;
        entity.UpdateSocialNetworks(
            (ValueObjectList<SocialNetwork>)socialNetworkBufferList);

        // handle BL
        var response = await _volunteerRepository.UpdateAsync(entity, cancellationToken);
        //if (createResponse.IsFailure) return createResponse.Error;

        _logger.LogInformation("Social networks for volunteer with id {Id} was updated", volunteerId.Value);

        return response;
    }
}
