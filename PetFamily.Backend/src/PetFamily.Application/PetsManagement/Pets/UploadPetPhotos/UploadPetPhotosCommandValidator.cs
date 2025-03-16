using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

namespace PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;
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
