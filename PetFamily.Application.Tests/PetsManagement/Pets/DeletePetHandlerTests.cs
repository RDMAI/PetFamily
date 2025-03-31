using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.Commands.AddPet;
using PetFamily.Application.PetsManagement.Pets.Commands.DeletePet;
using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class DeletePetHandlerTests : BaseHandlerTests
{
    public DeletePetHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_SoftDeletingPetWithoutPhotosWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        context.Species.Add(species);

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);
        volunteer.AddPet(pet);
        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        var command = new DeletePetCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, DeletePetCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is soft deleted in the database
        var entity = await context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        Assert.True(entity.Pets.Any(p => p.Id == result.Value && p.IsDeleted), "Pet is not softdeleted");
    }
}
