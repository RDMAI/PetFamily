using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Pets.Commands.SetMainPetPhoto;
public class SetMainPetPhotoCommandValidator : AbstractValidator<SetMainPetPhotoCommand>
{
    public SetMainPetPhotoCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("PetId"));

        RuleFor(c => c.PhotoPath)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Path"));
    }
}
