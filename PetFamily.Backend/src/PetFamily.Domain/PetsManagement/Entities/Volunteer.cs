using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Primitives;
using PetFamily.Domain.Shared.ValueObjects;
using System.Linq.Expressions;

namespace PetFamily.Domain.PetsManagement.Entities;

public class Volunteer : SoftDeletableEntity<VolunteerId>
{
    // EF Core
    private Volunteer() { }

    public Volunteer(VolunteerId id,
        VolunteerFullName fullName,
        Email email,
        Description description,
        VolunteerExperienceYears experienceYears,
        Phone phone,
        ValueObjectList<Requisites> requisites,
        ValueObjectList<SocialNetwork> socialNetworks) : base(id)
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
    public ValueObjectList<Requisites> Requisites { get; private set; }
    public ValueObjectList<SocialNetwork> SocialNetworks { get; private set; }

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
        ValueObjectList<SocialNetwork> socialNetworks)
    {
        SocialNetworks = socialNetworks;
        return this;
    }

    public Volunteer UpdateRequisites(
        ValueObjectList<Requisites> requisites)
    {
        Requisites = requisites;
        return this;
    }

    public override void Delete()
    {
        base.Delete();

        foreach (var pet in _pets)
            pet.Delete();
    }

    public override void Restore()
    {
        base.Restore();

        foreach (var pet in _pets)
            pet.Restore();
    }

    public Result<Volunteer, Error> AddPet(Pet pet)
    {
        if (pet is null)
            return ErrorHelper.General.ValueIsNullOrEmpty("Pet");

        var serialNumberResult = PetSerialNumber.Create(_pets.Count + 1);
        if (serialNumberResult.IsFailure)
            return ErrorHelper.General.MethodNotApplicable(
                "Cannot move pet. New position is invalid");

        pet.SetSerialNumber(serialNumberResult.Value);

        _pets.Add(pet);
        return this;
    }

    public UnitResult<Error> MovePet(PetId petId, PetSerialNumber newNumber)
    {
        if (_pets.Count == 1)
            return ErrorHelper.General.MethodNotApplicable(
                "Cannot move pet. List of pets contains only one pet.");

        var petCurrentNumber = _pets.FirstOrDefault(p => p.Id == petId)?.SerialNumber;
        if (petCurrentNumber is null)
            return ErrorHelper.General.NotFound(petId.Value);

        if (petCurrentNumber == newNumber)
            return ErrorHelper.General.MethodNotApplicable(
                "Cannot move pet. Old and new positions are the same.");

        // defining borders in pets list, where we will move pets
        (PetSerialNumber first, PetSerialNumber last) =
            petCurrentNumber > newNumber ?
            (newNumber, petCurrentNumber) :
            (petCurrentNumber, newNumber);

        var petsToMove = _pets.Where(p =>
                p.SerialNumber >= first && p.SerialNumber <= last)
                .OrderBy(p => p.SerialNumber.Value);

        foreach (var pet in petsToMove)
        {
            // moving specified pet to new specified position
            if (pet.SerialNumber == petCurrentNumber)
            {
                pet.SetSerialNumber(newNumber);
                continue;
            }

            // getting new positions for the rest of the pets
            var newValue = pet.SerialNumber.Value;
            if (petCurrentNumber > newNumber) newValue++;  // moving right
            else newValue--;  // moving left

            var n = PetSerialNumber.Create(newValue).Value;
            pet.SetSerialNumber(n);
        }

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> MovePetToStart(PetId petId)
    {
        var res = PetSerialNumber.Create(1);
        if (res.IsFailure)
            throw new ApplicationException("Could not create PetSerialNumber");
        var startPosition = res.Value;

        return MovePet(petId, startPosition);
    }

    public UnitResult<Error> MovePetToEnd(PetId petId)
    {
        var res = PetSerialNumber.Create(_pets.Count);
        if (res.IsFailure)
            throw new ApplicationException("Could not create PetSerialNumber");
        var endPosition = res.Value;

        return MovePet(petId, endPosition);
    }

    public UnitResult<Error> AddPhotosToPet(PetId petId, IEnumerable<FileVO> photos)
    {
        if (_pets.Count == 0)
            return ErrorHelper.General.MethodNotApplicable(
                "Volunteer does not have any pets");

        var pet = _pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return ErrorHelper.General.NotFound(petId.Value);

        // creating new objects to overcome ef core's tracking
        if (pet.Photos.Values is null)
        {
            pet.UpdatePhotos(new ValueObjectList<FileVO>(photos));
        }
        else
        {
            var petPhotos = pet.Photos.Values
            .Select(p => FileVO.Create(p.PathToStorage, p.Name).Value)
            .ToList();
            petPhotos.AddRange(photos);

            var petPhotosVO = new ValueObjectList<FileVO>(petPhotos);
            pet.UpdatePhotos(petPhotosVO);
        }

        return UnitResult.Success<Error>();
    }

    public int HardDeletePets(Func<Pet, bool> condition)
    {
        return _pets.RemoveAll(p => condition(p));
    }

    public UnitResult<Error> DeletePhotosFromPet(PetId petId, IEnumerable<string> photos)
    {
        if (_pets.Count == 0)
            return ErrorHelper.General.MethodNotApplicable(
                "Volunteer does not have any pets");

        var pet = _pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return ErrorHelper.General.NotFound(petId.Value);

        if (pet.Photos.Values is null)
            return ErrorHelper.General.NotFound();

        // creating new objects to overcome ef core's tracking
        var petPhotos = pet.Photos.Values
            .Select(p => FileVO.Create(p.PathToStorage, p.Name).Value)
            .ToList();
        var result = petPhotos.RemoveAll(f => photos.Contains(f.PathToStorage));
        if (result == 0)
            return ErrorHelper.General.NotFound();

        var petPhotosVO = new ValueObjectList<FileVO>(petPhotos);
        pet.UpdatePhotos(petPhotosVO);

        return UnitResult.Success<Error>();
    }
}
