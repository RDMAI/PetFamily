using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Pets.Commands.DeletePetPhotos;
public class DeletePetPhotosCommandValidator : AbstractValidator<DeletePetPhotosCommand>
{
    public DeletePetPhotosCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("PetId"));

        RuleFor(c => c.PhotoPaths)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Photos"));
    }
}
