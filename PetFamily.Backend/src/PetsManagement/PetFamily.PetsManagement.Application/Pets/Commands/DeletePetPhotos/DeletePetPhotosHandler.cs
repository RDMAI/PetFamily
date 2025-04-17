using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Files.Contracts;
using PetFamily.Files.Contracts.Requests;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Files;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.PetsManagement.Application.Pets.Commands.DeletePetPhotos;
public class DeletePetPhotosHandler
    : ICommandHandler<PetId, DeletePetPhotosCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IFileContract _fileProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeletePetPhotosCommandValidator _validator;
    private readonly ILogger<DeletePetPhotosHandler> _logger;

    public DeletePetPhotosHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IFileContract fileProvider,
        [FromKeyedServices(DependencyKey.Pets)] IUnitOfWork unitOfWork,
        DeletePetPhotosCommandValidator validator,
        ILogger<DeletePetPhotosHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _fileProvider = fileProvider;
        _unitOfWork = unitOfWork;
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
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);
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
                request: new DeleteFilesRequest(photoInfos),
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
