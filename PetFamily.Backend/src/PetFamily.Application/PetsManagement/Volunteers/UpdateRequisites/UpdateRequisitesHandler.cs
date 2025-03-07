using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.UpdateRequisites;
public class UpdateRequisitesHandler
{
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly UpdateRequisitesCommandValidator _validator;
    private readonly ILogger<UpdateRequisitesHandler> _logger;

    public UpdateRequisitesHandler(
        IVolunteerRepository volunteerRepository,
        UpdateRequisitesCommandValidator validator,
        ILogger<UpdateRequisitesHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<VolunteerId, ErrorList>> HandleAsync(
        UpdateRequisitesCommand command,
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
        List<Requisites> requisitesBufferList = [];
        foreach (RequisitesDTO requisites in command.RequisitesList)
        {
            requisitesBufferList.Add(Requisites.Create(requisites.Name,
                requisites.Description,
                requisites.Value).Value);
        }
        var requisitesList = RequisitesList.Create(requisitesBufferList).Value;

        // update entity
        var entity = entityResult.Value;
        entity.UpdateRequisites(requisitesList);

        // handle BL
        var response = await _volunteerRepository.UpdateAsync(entity, cancellationToken);
        //if (createResponse.IsFailure) return createResponse.Error;

        _logger.LogInformation("Requisites for volunteer with id {Id} was updated", volunteerId.Value);

        return response;
    }
}
