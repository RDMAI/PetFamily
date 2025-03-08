using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Tests.Helpers;

namespace PetFamily.Tests.Domain;

public class PetTests
{
    [Fact]
    public void AddPet_AddingFirstPet_Result_Success()
    {
        // Arrange
        var volunteer = EntitiesHelper.CreateValidVolunteer();
        var pet = EntitiesHelper.CreateValidPet();

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
        var volunteer = EntitiesHelper.CreateValidVolunteer();
        IEnumerable<Pet> pets = [
            EntitiesHelper.CreateValidPet("pet1"),
            EntitiesHelper.CreateValidPet("pet2"),
            EntitiesHelper.CreateValidPet("pet3"),
            EntitiesHelper.CreateValidPet("pet4"),
            EntitiesHelper.CreateValidPet("pet5"),
            EntitiesHelper.CreateValidPet("pet6")];

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        Pet petToMove = pets.First(p => p.Name.Value == "pet3");
        var newPosition = PetSerialNumber.Create(5).Value;

        IEnumerable<string> expectedOrder = ["pet1", "pet2", "pet4", "pet5", "pet3", "pet6"];

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
        var volunteer = EntitiesHelper.CreateValidVolunteer();
        IEnumerable<Pet> pets = [
            EntitiesHelper.CreateValidPet("pet1"),
            EntitiesHelper.CreateValidPet("pet2"),
            EntitiesHelper.CreateValidPet("pet3"),
            EntitiesHelper.CreateValidPet("pet4"),
            EntitiesHelper.CreateValidPet("pet5"),
            EntitiesHelper.CreateValidPet("pet6")];

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        Pet petToMove = pets.First(p => p.Name.Value == "pet4");
        var newPosition = PetSerialNumber.Create(2).Value;

        List<string> expectedOrder = ["pet1", "pet4", "pet2", "pet3", "pet5", "pet6"];

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
        var volunteer = EntitiesHelper.CreateValidVolunteer();
        IEnumerable<Pet> pets = [
            EntitiesHelper.CreateValidPet("pet1"),
            EntitiesHelper.CreateValidPet("pet2"),
            EntitiesHelper.CreateValidPet("pet3"),
            EntitiesHelper.CreateValidPet("pet4"),
            EntitiesHelper.CreateValidPet("pet5"),
            EntitiesHelper.CreateValidPet("pet6")];

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        Pet petToMove = pets.First(p => p.Name.Value == "pet1");

        IEnumerable<string> expectedOrder = ["pet2", "pet3", "pet4", "pet5", "pet6", "pet1"];

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
        var volunteer = EntitiesHelper.CreateValidVolunteer();
        IEnumerable<Pet> pets = [
            EntitiesHelper.CreateValidPet("pet1"),
            EntitiesHelper.CreateValidPet("pet2"),
            EntitiesHelper.CreateValidPet("pet3"),
            EntitiesHelper.CreateValidPet("pet4"),
            EntitiesHelper.CreateValidPet("pet5"),
            EntitiesHelper.CreateValidPet("pet6")];

        foreach (var pet in pets)
            volunteer.AddPet(pet);

        Pet petToMove = pets.First(p => p.Name.Value == "pet5");

        IEnumerable<string> expectedOrder = ["pet5", "pet1", "pet2", "pet3", "pet4", "pet6"];

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