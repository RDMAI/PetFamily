using CSharpFunctionalExtensions;
using PetFamily.Domain.PetsContext.ValueObjects.Pets;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesContext.Entities;
using System.Collections.Generic;

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

    public int PetsFoundHome => _pets.Where(d => d.Status.Value == PetStatus.Statuses.FoundHome).Count();
    public int PetsSeekingHome => _pets.Where(d => d.Status.Value == PetStatus.Statuses.SeekingHome).Count();
    public int PetsNeedHelp => _pets.Where(d => d.Status.Value == PetStatus.Statuses.NeedsHelp).Count();

    private Volunteer(VolunteerId id,
        VolunteerFullName fullName,
        Email email,
        string description,
        double experienceYears,
        Phone phone,
        Requisites requisites,
        SocialNetwork socialNetwork,
        List<Pet> pets) : base(id)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        Description = description;
        ExperienceYears = experienceYears;
        Phone = phone;
        Requisites = requisites;
        SocialNetwork = socialNetwork;
        _pets = pets;
    }

    public static Result<Volunteer> Create(VolunteerId id,
        VolunteerFullName fullName,
        Email email,
        string description,
        double experienceYears,
        Phone phone,
        Requisites requisites,
        SocialNetwork socialNetwork,
        List<Pet> pets)
    {
        if (string.IsNullOrWhiteSpace(description)) return Result.Failure<Volunteer>("Description cannot be empty");
        if (experienceYears <= 0) return Result.Failure<Volunteer>("Volunteer\'s experience should be greater than 0");

        return Result.Success(new Volunteer(
            id,
            fullName,
            email,
            description,
            experienceYears,
            phone,
            requisites,
            socialNetwork,
            pets));
    }

    public Result<Volunteer> AddPet(Pet pet)
    {
        if (pet is null) return Result.Failure<Volunteer>("Pet cannot be empty");

        _pets.Add(pet);
        return Result.Success(this);
    }
}
