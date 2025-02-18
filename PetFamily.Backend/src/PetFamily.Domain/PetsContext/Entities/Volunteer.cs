using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsContext.ValueObjects.Pets;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using System.Xml.Linq;

namespace PetFamily.Domain.PetsContext.Entities;

public class Volunteer : Entity<VolunteerId>
{
    public VolunteerFullName FullName { get; private set; }
    public Email Email { get; private set; }
    public string Description { get; private set; }
    public double ExperienceYears { get; private set; }
    public Phone Phone { get; private set; }
    public Requisites Requisites { get; private set; }
    public SocialNetwork SocialNetwork { get; private set; }

    public IReadOnlyList<Pet> Pets => _pets;
    private List<Pet> _pets = [];

    public int PetsFoundHome => _pets.Where(d => d.Status.Value == PetStatuses.FoundHome).Count();
    public int PetsSeekingHome => _pets.Where(d => d.Status.Value == PetStatuses.SeekingHome).Count();
    public int PetsNeedHelp => _pets.Where(d => d.Status.Value == PetStatuses.NeedsHelp).Count();

    // EF Core
    private Volunteer() { }
    private Volunteer(VolunteerId id,
        VolunteerFullName fullName,
        Email email,
        string description,
        double experienceYears,
        Phone phone,
        Requisites requisites,
        SocialNetwork socialNetwork) : base(id)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        Description = description;
        ExperienceYears = experienceYears;
        Phone = phone;
        Requisites = requisites;
        SocialNetwork = socialNetwork;
    }

    public static Result<Volunteer, Error> Create(VolunteerId id,
        VolunteerFullName fullName,
        Email email,
        string description,
        double experienceYears,
        Phone phone,
        Requisites requisites,
        SocialNetwork socialNetwork)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Length > Constants.MAX_HIGH_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Description");
        if (experienceYears <= 0)
            return ErrorHelper.General.ValueIsInvalid("Experience");

        return new Volunteer(
            id,
            fullName,
            email,
            description,
            experienceYears,
            phone,
            requisites,
            socialNetwork);
    }

    public Result<Volunteer, Error> AddPet(Pet pet)
    {
        if (pet is null)
            return ErrorHelper.General.ValueIsNullOrEmpty("Pet");

        _pets.Add(pet);
        return this;
    }
}
