using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Pets.Commands.UpdatePetStatus;

public class UpdatePetStatusCommandValidator : AbstractValidator<UpdatePetStatusCommand>
{
    public UpdatePetStatusCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("PetId"));

        RuleFor(c => c.Status).MustBeValueObject(CreatePetStatusFromDTO);
    }

    private Result<PetStatus, Error> CreatePetStatusFromDTO(int status)
        => PetStatus.Create((PetStatuses)status);
}
