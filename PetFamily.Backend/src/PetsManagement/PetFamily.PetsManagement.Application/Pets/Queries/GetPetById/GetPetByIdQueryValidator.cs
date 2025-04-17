using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPetById;

public class GetPetByIdQueryValidator : AbstractValidator<GetPetByIdQuery>
{
    public GetPetByIdQueryValidator()
    {
        RuleFor(q => q.PetId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsInvalid("Id"));
    }
}
