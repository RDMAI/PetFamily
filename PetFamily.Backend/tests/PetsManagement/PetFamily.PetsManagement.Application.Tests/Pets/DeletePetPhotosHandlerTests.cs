using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Pets.Commands.DeletePetPhotos;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Pets;

public class DeletePetPhotosHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<PetId, DeletePetPhotosCommand> sut;

    public DeletePetPhotosHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, DeletePetPhotosCommand>>();
    }

    [Fact]
    public async Task HandleAsync_DeletingPetPhotosWithValidCommand_ReturnSuccess()
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
        
        var pathToFile = Guid.NewGuid().ToString() + "_test.jpg";
        IEnumerable<FileVO> files = [FileVO.Create(pathToFile, "test.jpg").Value];
        volunteer.AddPet(pet);

        volunteer.AddPhotosToPet(pet.Id, files);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var command = new DeletePetPhotosCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value,
            PhotoPaths: [pathToFile]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is still in the database
        var entity = await _petsContext.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        var photos = entity.Pets.First().Photos.ToList();
        Assert.True(photos.Count == 0, "Photo is not deleted");
    }

    [Fact]
    public async Task HandleAsync_TryingToDeletePhotosFromNonexistantVolunteer_ReturnSuccess()
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

        var pathToFile = Guid.NewGuid().ToString() + "_test.jpg";
        IEnumerable<FileVO> files = [FileVO.Create(pathToFile, "test.jpg").Value];
        volunteer.AddPet(pet);
        volunteer.AddPhotosToPet(pet.Id, files);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var fakeVolunteerId = VolunteerId.GenerateNew();

        var command = new DeletePetPhotosCommand(
            VolunteerId: fakeVolunteerId.Value,
            PetId: pet.Id.Value,
            PhotoPaths: [pathToFile]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure, "Returned success, expected failure");
        Assert.True(result.Error is not null, "Result error is null");

        // check if the entity is in the database
        var petId = PetId.Create(command.PetId);
        var entity = await _petsContext.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == petId)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        var photos = entity.Pets.First().Photos.ToList();
        Assert.True(photos.Count != 0, "Photo is not deleted");
    }
}
