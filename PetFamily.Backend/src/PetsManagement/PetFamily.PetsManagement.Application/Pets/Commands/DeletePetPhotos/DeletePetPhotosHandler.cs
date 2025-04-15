using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.DeletePetPhotos;
public class DeletePetPhotosHandler
    : ICommandHandler<PetId, DeletePetPhotosCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IFileProvider _fileProvider;
    private readonly IUnitOfWork _transactionHelper;
    private readonly DeletePetPhotosCommandValidator _validator;
    private readonly ILogger<DeletePetPhotosHandler> _logger;

    public DeletePetPhotosHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IFileProvider fileProvider,
        IUnitOfWork transactionHelper,
        DeletePetPhotosCommandValidator validator,
        ILogger<DeletePetPhotosHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _fileProvider = fileProvider;
        _transactionHelper = transactionHelper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        DeletePetPhotosCommand command,
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

        var photoInfos = command.PhotoPaths.Select(p =>
            new FileInfoDTO(p, Constants.BucketNames.PET_PHOTOS));

        // handling BL
        var transaction = await _transactionHelper.BeginTransaction(cancellationToken);
        try
        {
            // delete photos' paths from DB
            var domainResult = volunteer.DeletePhotosFromPet(petId, command.PhotoPaths);
            //if (domainResult.IsFailure)
            //    return domainResult.Error.ToErrorList();

            var dbResult = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
            if (dbResult.IsFailure)
            {
                transaction.Rollback();
                return dbResult.Error;
            }

            // delete photos from file storage
            var fileStorageResult = await _fileProvider.DeleteFilesAsync(
                photoInfos,
                cancellationToken);

            if (fileStorageResult.IsFailure)
            {
                transaction.Rollback();
                return fileStorageResult.Error;
            }

            transaction.Commit();

            return petId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Cannot delete photos from pet with id = {id}. Rolling back transaction.",
                command.PetId);
            transaction.Rollback();

            return ErrorHelper.Files.DeleteFailure("Cannot delete photos from pet.").ToErrorList();
        }
    }
}
