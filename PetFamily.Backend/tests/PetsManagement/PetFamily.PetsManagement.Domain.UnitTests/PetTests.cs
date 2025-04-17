using PetFamily.PetsManagement.Domain.Entities;
using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Domain.UnitTests;

public class PetTests
{
    [Fact]
    public void AddPet_AddingFirstPet_Result_Success()
    {
        // Arrange
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(
            breedId: breed.Id,
            speciesId: species.Id);

        // Act
        var result = volunteer.AddPet(pet);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.Pets);
        Assert.True(volunteer.Pets.First().Id == pet.Id);
        Assert.True(result.Value.Pets.First().Id == pet.Id);
    }

    [Fact]
    public void MovePet_List6PetsMovingPet3To5_UnitResultSuccess()
    {
        // Arrange
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(
            breedId: breed.Id,
            speciesId: species.Id,
            amount: 6);

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        Pet petToMove = pets.First(p => p.Name.Value == "Fido_2");
        var newPosition = PetSerialNumber.Create(5).Value;

        IEnumerable<string> expectedOrder = ["Fido_0", "Fido_1", "Fido_3", "Fido_4", "Fido_2", "Fido_5"];

        // Act
        var result = volunteer.MovePet(petToMove.Id, newPosition);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(volunteer.Pets);
        Assert.True(_comparePetsListToExpected(volunteer.Pets, expectedOrder));
    }

    [Fact]
    public void MovePet_List6PetsMovingPet4To2_UnitResultSuccess()
    {
        // Arrange
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(
            breedId: breed.Id,
            speciesId: species.Id,
            amount: 6);

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        Pet petToMove = pets.First(p => p.Name.Value == "Fido_3");
        var newPosition = PetSerialNumber.Create(2).Value;

        List<string> expectedOrder = ["Fido_0", "Fido_3", "Fido_1", "Fido_2", "Fido_4", "Fido_5"];

        // Act
        var result = volunteer.MovePet(petToMove.Id, newPosition);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(volunteer.Pets);
        Assert.True(_comparePetsListToExpected(volunteer.Pets, expectedOrder));
    }

    [Fact]
    public void MovePetToEnd_List6PetsMovingPet1ToEnd_UnitResultSuccess()
    {
        // Arrange
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(
            breedId: breed.Id,
            speciesId: species.Id,
            amount: 6);

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        Pet petToMove = pets.First(p => p.Name.Value == "Fido_0");

        IEnumerable<string> expectedOrder = ["Fido_1", "Fido_2", "Fido_3", "Fido_4", "Fido_5", "Fido_0"];

        // Act
        var result = volunteer.MovePetToEnd(petToMove.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(volunteer.Pets);
        Assert.True(_comparePetsListToExpected(volunteer.Pets, expectedOrder));
    }

    [Fact]
    public void MovePetToEnd_List6PetsMovingPet5ToStart_UnitResultSuccess()
    {
        // Arrange
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(
            breedId: breed.Id,
            speciesId: species.Id,
            amount: 6);

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        Pet petToMove = pets.First(p => p.Name.Value == "Fido_4");

        IEnumerable<string> expectedOrder = ["Fido_4", "Fido_0", "Fido_1", "Fido_2", "Fido_3", "Fido_5"];

        // Act
        var result = volunteer.MovePetToStart(petToMove.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(volunteer.Pets);
        Assert.True(_comparePetsListToExpected(volunteer.Pets, expectedOrder));
    }

    private bool _comparePetsListToExpected(IEnumerable<Pet> pets, IEnumerable<string> expectedOrder)
    {
        var orderedNames = pets
            .OrderBy(p => p.SerialNumber.Value)
            .Select(p => p.Name.Value);

        for (var i = 0; i < orderedNames.Count(); i++)
        {
            if (orderedNames.ElementAt(i) != expectedOrder.ElementAt(i))
                return false;
        }

        return true;
    }
}