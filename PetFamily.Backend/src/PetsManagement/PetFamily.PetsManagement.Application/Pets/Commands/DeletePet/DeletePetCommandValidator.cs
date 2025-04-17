using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Pets.Commands.DeletePet;

public class DeletePetCommandValidator : AbstractValidator<DeletePetCommand>
{
    public DeletePetCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("PetId"));
    }
}
