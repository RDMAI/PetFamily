using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.Shared.Kernel.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Domain.Entities;
public class Pet : SoftDeletableEntity<PetId>
{
    // EF Core
    private Pet() { }

    public Pet(PetId id,
        PetName name,
        Description description,
        PetColor color,
        PetWeight weight,
        PetHeight height,
        PetBreed breed,
        PetHealthInfo healthInformation,
        Address address,
        Phone ownerPhone,
        bool isCastrated,
        DateOnly birthDate,
        bool isVacinated,
        PetStatus status,
        ValueObjectList<Requisites> requisites) : base(id)
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

    public PetName Name { get; private set; }
    public Description Description { get; private set; }
    public PetColor Color { get; private set; }
    public PetWeight Weight { get; private set; }
    public PetHeight Height { get; private set; }
    public PetBreed Breed { get; private set; }
    public PetHealthInfo HealthInformation { get; private set; }
    public Address Address { get; private set; }
    public Phone OwnerPhone { get; private set; }
    public bool IsCastrated { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public bool IsVacinated { get; private set; }
    public PetStatus Status { get; private set; }  // Pet's status - needs help / seeks home / found home
    public ValueObjectList<FileVO> Photos { get; private set; } = new([]);
    public ValueObjectList<Requisites> Requisites { get; private set; }
    public DateTime CreationDate = DateTime.Now;

    public PetSerialNumber SerialNumber { get; private set; }

    public void SetSerialNumber(PetSerialNumber serialNumber)
    {
        SerialNumber = serialNumber;
    }

    public Pet UpdatePhotos(ValueObjectList<FileVO> photos)
    {
        Photos = photos;
        return this;
    }

    public Pet UpdateFull(PetName name,
        Description description,
        PetColor color,
        PetWeight weight,
        PetHeight height,
        PetBreed breed,
        PetHealthInfo healthInformation,
        Address address,
        Phone ownerPhone,
        bool isCastrated,
        DateOnly birthDate,
        bool isVacinated,
        PetStatus status,
        ValueObjectList<Requisites> requisites)
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

        return this;
    }

    public Pet UpdateStatus(PetStatus status)
    {
        Status = status;
        return this;
    }

    public Pet SetMainPhoto()
    {
        return this;
    }
}
