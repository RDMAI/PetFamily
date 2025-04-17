using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Pets.Commands.UploadPetPhotos;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Files;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Pets;

public class UploadPetPhotosHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<PetId, UploadPetPhotosCommand> _sut;

    public UploadPetPhotosHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, UploadPetPhotosCommand>>();
    }

    [Fact]
    public async Task HandleAsync_UploadPetPhotosWithValidCommand_ReturnSuccess()
    {
        // Arrange
        _webFactory.SetupSuccessFileProviderMock();

        // seed database
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);
        volunteer.AddPet(pet);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        using Stream stream = new MemoryStream();

        IEnumerable<FileDTO> fileDTOs = [new FileDTO(
            ContentStream: stream,
            NameWithExtenson: "Test_1.jpg")];

        var command = new UploadPetPhotosCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value,
            Photos: fileDTOs);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is still in the database
        var entity = await _petsContext.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        var photos = entity.Pets.First().Photos.ToList();
        Assert.True(photos.Count == 1, "Photo is not uploaded");
    }
}
