using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

namespace PetFamily.Application.PetsManagement.Pets.Queries.GetPetById;

public class GetPetByIdQueryValidator : AbstractValidator<GetPetByIdQuery>
{
    public GetPetByIdQueryValidator()
    {
        RuleFor(q => q.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsInvalid("Id"));
    }
}
