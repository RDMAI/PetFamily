using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPetPhotos;
public class GetPetPhotosHandler
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IFileProvider _fileProvider;
    private readonly IUnitOfWork _transactionHelper;
    private readonly GetPetPhotosCommandValidator _validator;
    private readonly ILogger<GetPetPhotosHandler> _logger;

    public GetPetPhotosHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IFileProvider fileProvider,
        IUnitOfWork transactionHelper,
        GetPetPhotosCommandValidator validator,
        ILogger<GetPetPhotosHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _fileProvider = fileProvider;
        _transactionHelper = transactionHelper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<string>, ErrorList>> HandleAsync(
        GetPetPhotosCommand command,
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
        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == petId);
        if (pet is null)
            return ErrorHelper.General.NotFound(petId.Value).ToErrorList();

        if (pet.Photos.Values is null || pet.Photos.Count == 0)
            return ErrorHelper.General.MethodNotApplicable("Pet does not have any photos").ToErrorList();

        var photoInfos = pet.Photos.Select(p => new FileInfoDTO(
            p.PathToStorage,
            Constants.BucketNames.PET_PHOTOS));

        try
        {
            var photosLinksResult = await _fileProvider.GetFilesAsync(
                photoInfos,
                cancellationToken);

            return photosLinksResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Cannot add photos to pet with id = {id}. Rolling back transaction.",
                command.PetId);

            return ErrorHelper.Files.GetFailure("Cannot get photos from pet.").ToErrorList();
        }
    }
}
