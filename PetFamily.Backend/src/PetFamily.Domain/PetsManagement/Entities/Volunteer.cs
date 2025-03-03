using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsContext.ValueObjects.Pets;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Domain.PetsContext.Entities;

public class Volunteer : Entity<VolunteerId>
{
    // EF Core
    private Volunteer() { }
    public Volunteer(VolunteerId id,
        VolunteerFullName fullName,
        Email email,
        Description description,
        VolunteerExperienceYears experienceYears,
        Phone phone,
        RequisitesList requisites,
        SocialNetworkList socialNetworks) : base(id)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        Description = description;
        ExperienceYears = experienceYears;
        Phone = phone;
        Requisites = requisites;
        SocialNetworks = socialNetworks;
    }

    public VolunteerFullName FullName { get; private set; }
    public Email Email { get; private set; }
    public Description Description { get; private set; }
    public VolunteerExperienceYears ExperienceYears { get; private set; }
    public Phone Phone { get; private set; }
    public RequisitesList Requisites { get; private set; }
    public SocialNetworkList SocialNetworks { get; private set; }

    public IReadOnlyList<Pet> Pets => _pets;
    private List<Pet> _pets = [];

    public int PetsFoundHome => _pets.Where(d => d.Status.Value == PetStatuses.FoundHome).Count();
    public int PetsSeekingHome => _pets.Where(d => d.Status.Value == PetStatuses.SeekingHome).Count();
    public int PetsNeedHelp => _pets.Where(d => d.Status.Value == PetStatuses.NeedsHelp).Count();

    public Volunteer UpdateMainInfo(
        VolunteerFullName fullName,
        Email email,
        Description description,
        VolunteerExperienceYears experienceYears,
        Phone phone)
    {
        FullName = fullName;
        Email = email;
        Description = description;
        ExperienceYears = experienceYears;
        Phone = phone;
        return this;
    }

    public Volunteer UpdateSocialNetworks(
        SocialNetworkList socialNetworks)
    {
        SocialNetworks = socialNetworks;
        return this;
    }

    public Volunteer UpdateRequisites(
        RequisitesList requisites)
    {
        Requisites = requisites;
        return this;
    }

    public Result<Volunteer, Error> AddPet(Pet pet)
    {
        if (pet is null)
            return ErrorHelper.General.ValueIsNullOrEmpty("Pet");

        _pets.Add(pet);
        return this;
    }
}
