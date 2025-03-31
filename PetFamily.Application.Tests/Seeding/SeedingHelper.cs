using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared.Primitives;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesManagement.Entities;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.IntegrationTests.Seeding;
public static class SeedingHelper
{
    public static Volunteer CreateValidVolunteer(
        string specificFirstName = "Ivan",
        string uniqueEmail = "Example@gmail.com",
        string uniquePhone = "89000000000")
    {
        var volunteerId = VolunteerId.GenerateNew();
        var fullName = VolunteerFullName.Create(
            specificFirstName,
            "Ivanov",
            "Ivanovich").Value;

        var email = Email.Create(uniqueEmail).Value;
        var description = Description.Create("Test description for Ivanov Ivan Ivanovich").Value;
        var experience = VolunteerExperienceYears.Create(10).Value;
        var phone = Phone.Create(uniquePhone).Value;

        IEnumerable<Requisites> requisitesEnumerable = [
            Requisites.Create("SPB", "Test SPB description", phone.Value).Value];
        var requisitesVO = new ValueObjectList<Requisites>(requisitesEnumerable);

        IEnumerable<SocialNetwork> socialNetworksEnumerable = [
            SocialNetwork.Create("VK", "https://vk.com/id000000000").Value];
        var socialNetworksVO = new ValueObjectList<SocialNetwork>(socialNetworksEnumerable);

        return new Volunteer(volunteerId,
            fullName,
            email,
            description,
            experience,
            phone,
            requisitesVO,
            socialNetworksVO);
    }

    /// <summary>
    /// Creates list of valid volunteers with unique phone and email.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static List<Volunteer> CreateValidVolunteerList(int amount)
    {
        List<Volunteer> volunteers = [];

        for (int i = 0; i < amount; i++)
        {
            var phone = string.Empty;
            if (amount < 10) phone = $"8900000000{amount}";
            else if (amount < 100) phone = $"890000000{amount}";
            else if (amount < 1000) phone = $"89000000{amount}";

            volunteers.Add(CreateValidVolunteer(
                specificFirstName: $"Ivan_{i}",
                uniqueEmail: $"Example{i}@gmail.com",
                uniquePhone: phone));
        }

        return volunteers;
    }

    public static Pet CreateValidPet(BreedId breedId, SpeciesId speciesId, string specificName = "Fido")
    {
        var petId = PetId.GenerateNew();
        var name = PetName.Create(specificName).Value;

        var description = Description.Create($"Test description for {name.Value}").Value;
        var color = PetColor.Create("Black").Value;
        var weight = PetWeight.Create(10).Value;
        var height = PetHeight.Create(30).Value;

        var breed = PetBreed.Create(breedId, speciesId).Value;

        var healthInfo = PetHealthInfo.Create("Healthy").Value;

        var address = Address.Create("Moscow",
            "Dvorovaya",
            1,
            null,
            1).Value;

        var phone = Phone.Create("89000000001").Value;
        var isCastrated = true;
        var birthDate = new DateOnly(2020, 01, 01);
        var isVacinated = true;
        var status = PetStatus.Create(PetStatuses.SeekingHome).Value;

        IEnumerable<Requisites> requisitesEnumerable = [
            Requisites.Create("SPB", "Test SPB description", phone.Value).Value];
        var requisitesVO = new ValueObjectList<Requisites>(requisitesEnumerable);

        return new Pet(petId,
            name,
            description,
            color,
            weight,
            height,
            breed,
            healthInfo,
            address,
            phone,
            isCastrated,
            birthDate,
            isVacinated,
            status,
            requisitesVO);
    }

    /// <summary>
    /// Creates list of valid pets with unique name (Fido + iteration counter)
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static List<Pet> CreateValidPetList(BreedId breedId, SpeciesId speciesId, int amount)
    {
        List<Pet> pets = [];

        for (int i = 0; i < amount; i++)
        {
            pets.Add(CreateValidPet(
                breedId: breedId,
                speciesId: speciesId,
                specificName: $"Fido_{i}"));
        }

        return pets;
    }

    public static Species CreateValidSpecies(string specificName = "Dog")
    {
        return new Species(
            id: SpeciesId.GenerateNew(),
            name: SpeciesName.Create(specificName).Value);
    }

    /// <summary>
    /// Creates list of valid 4 species: Dog, Cat, Parrot and Hamster
    /// </summary>
    /// <returns></returns>
    public static List<Species> CreateValidSpeciesList()
    {
        List<Species> species = [];
        species.Add(CreateValidSpecies(specificName: "Dog"));
        species.Add(CreateValidSpecies(specificName: "Cat"));
        species.Add(CreateValidSpecies(specificName: "Parrot"));
        species.Add(CreateValidSpecies(specificName: "Hamster"));

        return species;
    }

    public static Breed CreateValidBreed(string specificName = "Labrador")
    {
        return new Breed(
            id: BreedId.GenerateNew(),
            name: BreedName.Create(specificName).Value);
    }

    /// <summary>
    /// Creates list of valid pets with unique name (PetBreed + iteration counter)
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static List<Breed> CreateValidBreedList(int amount)
    {
        List<Breed> breeds = [];

        for (int i = 0; i < amount; i++)
        {
            breeds.Add(CreateValidBreed(specificName: $"PetBreed_{i}"));
        }

        return breeds;
    }
}
