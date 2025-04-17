using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Pets.Commands.MovePet;
using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Pets;

public class MovePetHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<PetId, MovePetCommand> _sut;

    public MovePetHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, MovePetCommand>>();
    }

    [Fact]
    public async Task HandleAsync_MovingPetFrom1To5_ReturnSuccess()
    {
        // Arrange
        // seed database
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(breed.Id, species.Id, 6);
        foreach (var pet in pets) volunteer.AddPet(pet);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var command = new MovePetCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pets[0].Id.Value,
            NewSerialNumber: 5);

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
        Assert.True(entity.Pets.Count == 6, "Pets list count is incorrect");

        var movedPet = entity.Pets.First(p => p.Id == result.Value);
        Assert.True(
            movedPet.SerialNumber == PetSerialNumber.Create(command.NewSerialNumber).Value,
            "Pet serial number is not correct");
    }
}
