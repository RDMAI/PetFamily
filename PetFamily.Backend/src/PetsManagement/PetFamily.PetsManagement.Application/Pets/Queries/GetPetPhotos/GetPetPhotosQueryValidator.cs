using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPetPhotos;
public class GetPetPhotosQueryValidator : AbstractValidator<GetPetPhotosQuery>
{
    public GetPetPhotosQueryValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("PetId"));
    }
}
