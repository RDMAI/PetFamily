﻿using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.DeleteVolunteer;
public class DeleteVolunteerHandler
    : ICommandHandler<VolunteerId, DeleteVolunteerCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly DeleteVolunteerCommandValidator _validator;
    private readonly ILogger<DeleteVolunteerHandler> _logger;

    public DeleteVolunteerHandler(
        IVolunteerAggregateRepository volunteerRepository,
        DeleteVolunteerCommandValidator validator,
        ILogger<DeleteVolunteerHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<VolunteerId, ErrorList>> HandleAsync(
        DeleteVolunteerCommand command,
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

        // mark entity as deleted
        var entity = entityResult.Value;
        entity.Delete();

        // handle BL
        var response = await _volunteerRepository.SoftDeleteAsync(entityResult.Value, cancellationToken);
        //if (createResponse.IsFailure) return createResponse.Error;

        _logger.LogInformation("Volunteer with id {Id} marked as deleted", volunteerId.Value);

        return response;
    }
}
