using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsContext.ValueObjects.Pets;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Domain.PetsContext.Entities;
public class Pet : Entity<PetId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    //public Photo? Photo { get; private set; }
    public PetColor Color { get; private set; }
    public float Weight { get; private set; }
    public float Height { get; private set; }
    public PetBreed Breed { get; private set; }
    public PetHealthInfo HealthInformation { get; private set; }
    public Address Address { get; private set; }
    public Phone OwnerPhone { get; private set; }
    public bool IsCastrated { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public bool IsVacinated { get; private set; }
    public PetStatus Status { get; private set; }  // Pet's status - needs help / seeks home / found home
    public Requisites Requisites { get; private set; }
    public DateTime CreationDate = DateTime.Now;

    // EF Core
    private Pet() { }

    private Pet(PetId id,
        string name,
        string description,
        PetColor color,
        float weight,
        float height,
        PetBreed breed,
        PetHealthInfo healthInformation,
        Address address,
        Phone ownerPhone,
        bool isCastrated,
        DateOnly birthDate,
        bool isVacinated,
        PetStatus status,
        Requisites requisites) : base(id)
    {
        Name = name;
        Description = description;
        Color = color;
        Weight = weight;
        Height = height;
        Breed = breed;
        HealthInformation = healthInformation;
        Address = address;
        OwnerPhone = ownerPhone;
        IsCastrated = isCastrated;
        BirthDate = birthDate;
        IsVacinated = isVacinated;
        Status = status;
        Requisites = requisites;
    }

    public static Result<Pet, Error> Create(PetId id,
        string name,
        string description,
        PetColor color,
        float weight,
        float height,
        PetBreed breed,
        PetHealthInfo healthInformation,
        Address address,
        Phone ownerPhone,
        bool isCastrated,
        DateOnly birthDate,
        bool isVacinated,
        PetStatus status,
        Requisites requisites)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Name");
        if (string.IsNullOrWhiteSpace(description) || description.Length > Constants.MAX_HIGH_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Description");
        if (weight <= 0)
            return ErrorHelper.General.ValueIsInvalid("Weight");
        if (height <= 0)
            return ErrorHelper.General.ValueIsInvalid("Height");

        return new Pet(
            id,
            name,
            description,
            color,
            weight,
            height,
            breed,
            healthInformation,
            address,
            ownerPhone,
            isCastrated,
            birthDate,
            isVacinated,
            status,
            requisites);
    }
}
