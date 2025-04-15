using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Files.Contracts;
using PetFamily.Files.Contracts.Requests;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPetPhotos;
public class GetPetPhotosHandler
    : IQueryHandler<IEnumerable<string>, GetPetPhotosQuery>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IFileContract _fileAPI;
    private readonly GetPetPhotosQueryValidator _validator;
    private readonly ILogger<GetPetPhotosHandler> _logger;

    public GetPetPhotosHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IFileContract fileAPI,
        GetPetPhotosQueryValidator validator,
        ILogger<GetPetPhotosHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _fileAPI = fileAPI;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<string>, ErrorList>> HandleAsync(
        GetPetPhotosQuery command,
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

        var photoInfos = pet.Photos.Select(p => new Shared.Core.Files.FileInfo(
            p.PathToStorage,
            Constants.BucketNames.PET_PHOTOS));

        try
        {
            var photosLinksResult = await _fileAPI.GetFilesAsync(
                request: new GetFilesRequest(photoInfos),
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
