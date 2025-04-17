using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UpdatePet;

public class UpdatePetCommandValidator : AbstractValidator<UpdatePetCommand>
{
    public UpdatePetCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("PetId"));

        RuleFor(c => c.Pet)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Pet"));

        RuleFor(c => c.Pet.Name).MustBeValueObject(PetName.Create);
        RuleFor(c => c.Pet.Description).MustBeValueObject(Description.Create);
        RuleFor(c => c.Pet.Color).MustBeValueObject(PetColor.Create);
        RuleFor(c => c.Pet.Weight).MustBeValueObject(PetWeight.Create);
        RuleFor(c => c.Pet.Height).MustBeValueObject(PetHeight.Create);

        RuleFor(c => c.Pet.BreedId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("BreedId"));

        RuleFor(c => c.Pet.HealthInformation)
            .MustBeValueObject(PetHealthInfo.Create);
        RuleFor(c => c.Pet.OwnerPhone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.Pet.Status).MustBeValueObject(CreatePetStatusFromDTO);

        RuleFor(c => c.Address)
            .MustBeValueObject(a => Address.Create(
                a.City,
                a.Street,
                a.HouseNumber,
                a.HouseSubNumber,
                a.AppartmentNumber));

        RuleFor(c => c.RequisitesList)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Requisites"));

        RuleForEach(c => c.RequisitesList).MustBeValueObject(CreateRequisitesFromDTO);
    }

    private Result<PetStatus, Error> CreatePetStatusFromDTO(int status)
        => PetStatus.Create((PetStatuses)status);

    private Result<Requisites, Error> CreateRequisitesFromDTO(RequisitesDTO dto)
        => Requisites.Create(dto.Name, dto.Description, dto.Value);
}
