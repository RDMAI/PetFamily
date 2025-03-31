using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.Commands.DeletePetPhotos;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class DeletePetPhotosHandlerTests : BaseHandlerTests
{
    public DeletePetPhotosHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_DeletingPetPhotosWithValidCommand_ReturnSuccess()
    {
        // Arrange
        _webFactory.SetupSuccessFileProviderMock();

        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        context.Species.Add(species);

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);
        
        var pathToFile = Guid.NewGuid().ToString() + "_test.jpg";
        IEnumerable<FileVO> files = [FileVO.Create(pathToFile, "test.jpg").Value];
        volunteer.AddPhotosToPet(pet.Id, files);

        volunteer.AddPet(pet);
        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        var command = new DeletePetPhotosCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value,
            PhotoPaths: [pathToFile]);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, DeletePetPhotosCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is still in the database
        var entity = await context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        var photos = entity.Pets.First().Photos.ToList();
        Assert.True(photos.Count == 0, "Photo is not deleted");
    }
}
