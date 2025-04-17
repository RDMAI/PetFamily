using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UpdatePetStatus;

public class UpdatePetStatusHandler
    : ICommandHandler<PetId, UpdatePetStatusCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly UpdatePetStatusCommandValidator _validator;
    private readonly ILogger<UpdatePetStatusHandler> _logger;

    public UpdatePetStatusHandler(
        IVolunteerAggregateRepository volunteerRepository,
        [FromKeyedServices(DependencyKey.Pets)] IDBConnectionFactory dBConnectionFactory,
        UpdatePetStatusCommandValidator validator,
        ILogger<UpdatePetStatusHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        UpdatePetStatusCommand command,
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

        // validate volunteer
        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;
        var volunteer = volunteerResult.Value;

        // validate pet
        var petId = PetId.Create(command.PetId);
        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return ErrorHelper.General.NotFound(petId.Value).ToErrorList();

        var petStatus = PetStatus.Create((PetStatuses)command.Status).Value;

        // Handle BL
        pet.UpdateStatus(petStatus);

        var result = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return petId;
    }
}
