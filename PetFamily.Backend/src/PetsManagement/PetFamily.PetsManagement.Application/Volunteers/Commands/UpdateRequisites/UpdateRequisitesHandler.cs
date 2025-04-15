using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateRequisites;
public class UpdateRequisitesHandler
    : ICommandHandler<VolunteerId, UpdateRequisitesCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly UpdateRequisitesCommandValidator _validator;
    private readonly ILogger<UpdateRequisitesHandler> _logger;

    public UpdateRequisitesHandler(
        IVolunteerAggregateRepository volunteerRepository,
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

        // update entity
        var entity = entityResult.Value;
        entity.UpdateRequisites(
            (ValueObjectList<Requisites>)requisitesBufferList);

        // handle BL
        var response = await _volunteerRepository.UpdateAsync(entity, cancellationToken);
        //if (createResponse.IsFailure) return createResponse.Error;

        _logger.LogInformation("Requisites for volunteer with id {Id} was updated", volunteerId.Value);

        return response;
    }
}
