using PetFamily.PetsManagement.Domain.Entities;
using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.PetsManagement.Domain.ValueObjects.Volunteers;
using PetFamily.Shared.Kernel.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Domain.Entities;
using PetFamily.SpeciesManagement.Domain.ValueObjects;

namespace PetFamily.Tests.Shared;

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

    private static readonly string[] petColors = ["Black", "White", "Brown", "Gray"];

    /// <summary>
    /// Creates valid pet with partially random values of properties
    /// </summary>
    /// <param name="breedId"></param>
    /// <param name="speciesId"></param>
    /// <param name="specificName"></param>
    /// <returns></returns>
    public static Pet CreateValidPet(BreedId breedId, SpeciesId speciesId, string specificName = "Fido")
    {
        Random rnd = new Random();

        var petId = PetId.GenerateNew();
        var name = PetName.Create(specificName).Value;

        var description = Description.Create($"Test description for {name.Value}").Value;

        var color = PetColor.Create(
            petColors[rnd.Next(0, petColors.Length - 1)]
            ).Value;
        var weight = PetWeight.Create(rnd.Next(1, 10)).Value;
        var height = PetHeight.Create(rnd.Next(1, 30)).Value;

        var breed = PetBreed.Create(breedId, speciesId).Value;

        var healthInfo = PetHealthInfo.Create(
            rnd.Next(100) > 50 ? "Healthy" : "Needs medical attention"
            ).Value;

        var address = Address.Create("Moscow",
            "Dvorovaya",
            rnd.Next(1, 60),
            null,
            rnd.Next(1, 20)).Value;

        var phone = Phone.Create("89000000" + rnd.Next(9) + rnd.Next(9) + rnd.Next(9)).Value;
        var isCastrated = rnd.Next(100) > 50;
        var birthDate = new DateOnly(
            year: 2000 + rnd.Next(25),
            month: rnd.Next(1, 12),
            day: rnd.Next(1, 28));
        var isVacinated = rnd.Next(100) > 50;
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
    /// Creates list of valid species with unique name (Species + iteration counter)
    /// </summary>
    /// <returns></returns>
    public static List<Species> CreateValidSpeciesList(int amount)
    {
        List<Species> species = [];

        for (int i = 0; i < amount; i++)
        {
            species.Add(CreateValidSpecies(specificName: $"Species_{i}"));
        }

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
