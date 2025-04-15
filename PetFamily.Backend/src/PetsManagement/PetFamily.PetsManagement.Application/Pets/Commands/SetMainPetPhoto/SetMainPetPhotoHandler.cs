using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Files.Contracts;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Application.Pets.Commands.SetMainPetPhoto;
public class SetMainPetPhotoHandler
    : ICommandHandler<PetId, SetMainPetPhotoCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly SetMainPetPhotoCommandValidator _validator;
    private readonly ILogger<SetMainPetPhotoHandler> _logger;

    public SetMainPetPhotoHandler(
        IVolunteerAggregateRepository volunteerRepository,
        SetMainPetPhotoCommandValidator validator,
        ILogger<SetMainPetPhotoHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        SetMainPetPhotoCommand command,
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

        var petId = PetId.Create(command.PetId);

        // handling BL
        var domainResult = volunteer.SetMainPetPhoto(petId, command.PhotoPath);
        if (domainResult.IsFailure)
            return domainResult.Error.ToErrorList();

        var dbResult = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
        if (dbResult.IsFailure)
            return dbResult.Error;

        return petId;
    }
}
