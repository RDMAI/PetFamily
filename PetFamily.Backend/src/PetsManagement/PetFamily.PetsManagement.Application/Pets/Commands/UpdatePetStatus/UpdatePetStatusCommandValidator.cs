using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UpdatePetStatus;

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
