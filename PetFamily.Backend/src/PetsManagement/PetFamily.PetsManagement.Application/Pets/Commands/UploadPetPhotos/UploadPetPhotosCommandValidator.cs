using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UploadPetPhotos;
public class UploadPetPhotosCommandValidator : AbstractValidator<UploadPetPhotosCommand>
{
    public UploadPetPhotosCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("PetId"));

        RuleFor(c => c.Photos)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Photos"));
    }
}
