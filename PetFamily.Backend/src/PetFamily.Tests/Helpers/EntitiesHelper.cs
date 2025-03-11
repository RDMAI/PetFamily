using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Tests.Helpers;
public static class EntitiesHelper
{
    /// <summary>
    /// Returns valid volunteer
    /// </summary>
    /// <returns></returns>
    public static Volunteer CreateValidVolunteer()
    {
        var volunteerId = VolunteerId.GenerateNew();
        var fullName = VolunteerFullName.Create("Ivan",
            "Ivanov",
            "Ivanovich").Value;

        var email = Email.Create("Example@gmail.com").Value;
        var description = Description.Create("Test description for Ivanov Ivan Ivanovich").Value;
        var experience = VolunteerExperienceYears.Create(10).Value;
        var phone = Phone.Create("89000000000").Value;

        IEnumerable<Requisites> requisitesEnumerable = [
            Requisites.Create("SPB", "Test SPB description", phone.Value).Value];
        var requisitesVO = RequisitesList.Create(requisitesEnumerable).Value;

        IEnumerable<SocialNetwork> socialNetworksEnumerable = [
            SocialNetwork.Create("VK", "https://vk.com/id000000000").Value];
        var socialNetworksVO = SocialNetworkList.Create(socialNetworksEnumerable).Value;

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
    /// Returns valid pet with specified or default name
    /// </summary>
    /// <param name="specificName"></param>
    /// <returns></returns>
    public static Pet CreateValidPet(string specificName = "Fido")
    {
        var petId = PetId.GenerateNew();
        var name = PetName.Create(specificName).Value;

        var description = Description.Create($"Test description for {name.Value}").Value;
        var color = PetColor.Create("Black").Value;
        var weight = PetWeight.Create(10).Value;
        var height = PetHeight.Create(30).Value;

        var breed = PetBreed.Create(BreedId.GenerateNew(), SpeciesId.GenerateNew()).Value;

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
        var requisitesVO = RequisitesList.Create(requisitesEnumerable).Value;


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
}
