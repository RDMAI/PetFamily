using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPetPhotos;
public class GetPetPhotosCommandValidator : AbstractValidator<GetPetPhotosCommand>
{
    public GetPetPhotosCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("PetId"));
    }
}
