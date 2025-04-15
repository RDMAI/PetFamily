using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.Commands.UpdatePet;
using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class UpdatePetHandlerTests : BaseHandlerTests
{
    public UpdatePetHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_UpdatePetNameWithValidCommand_ReturnSuccess()
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

        var petDTO = new UpdatePetDTO(
            Name: "UpdatedName",
            Description: pet.Description.Value,
            Color: pet.Color.Value,
            Weight: pet.Weight.Value,
            Height: pet.Height.Value,
            BreedId: pet.Breed.BreedId.Value,
            HealthInformation: pet.HealthInformation.Value,
            OwnerPhone: pet.OwnerPhone.Value,
            IsCastrated: pet.IsCastrated,
            BirthDate: pet.BirthDate,
            IsVacinated: pet.IsVacinated,
            Status: (int)pet.Status.Value);

        var addressDTO = new AddressDTO(
            City: "Moscow",
            Street: "test street",
            HouseNumber: 1,
            AppartmentNumber: 1);

        var command = new UpdatePetCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value,
            Pet: petDTO,
            Address: addressDTO,
            RequisitesList: [ new RequisitesDTO(
                Name: "SPB",
                Description: "Test SPB description",
                Value: "89000000000")]);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, UpdatePetCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.Pets is not null, "Pets list is null");
        Assert.True(entity.Pets.Count == 1, "Pets list count is incorrect");

        var updatedPet = entity.Pets[0];
        Assert.True(
            updatedPet.Name.Value == command.Pet.Name,
            $"Pet name is incorrect - {updatedPet.Name.Value}, should be {command.Pet.Name}");
    }
}
