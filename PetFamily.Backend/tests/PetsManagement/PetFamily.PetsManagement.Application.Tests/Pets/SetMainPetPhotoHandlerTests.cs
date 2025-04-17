using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Pets.Commands.SetMainPetPhoto;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

using FileVO = PetFamily.Shared.Kernel.ValueObjects.File;

namespace PetFamily.PetsManagement.Application.Tests.Pets;

public class SetMainPetPhotoHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<PetId, SetMainPetPhotoCommand> _sut;

    public SetMainPetPhotoHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, SetMainPetPhotoCommand>>();
    }

    [Fact]
    public async Task HandleAsync_SettingNewMainPhotoWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);
        volunteer.AddPet(pet);

        List<string> pathsToFiles = [
            Guid.NewGuid().ToString() + "_test1.jpg",
            Guid.NewGuid().ToString() + "_test2.jpg"];
        var files = pathsToFiles.Select(f => FileVO.Create(f, "test.jpg").Value);
        volunteer.AddPhotosToPet(pet.Id, files);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var command = new SetMainPetPhotoCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value,
            PhotoPath: pathsToFiles[1]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await _petsContext.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.Pets is not null, "Pets list is null");
        Assert.True(entity.Pets.Count == 1, "Pets list count is incorrect");

        var petFromDB = entity.Pets.First();
        Assert.True(petFromDB.Photos.Count == 2, "Pet photos amount is changed");

        var mainPhotoFromDB = petFromDB.Photos[0].PathToStorage;
        Assert.True(mainPhotoFromDB == command.PhotoPath, "Main photo is incorrect");
    }
}
