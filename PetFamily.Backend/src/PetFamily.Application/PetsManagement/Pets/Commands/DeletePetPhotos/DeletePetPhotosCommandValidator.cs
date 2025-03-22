using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

namespace PetFamily.Application.PetsManagement.Pets.Commands.DeletePetPhotos;
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
