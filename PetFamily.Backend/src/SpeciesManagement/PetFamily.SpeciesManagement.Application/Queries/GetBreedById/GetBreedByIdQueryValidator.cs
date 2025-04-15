using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.SpeciesManagement.Application.Queries.GetBreedById;

public class GetBreedByIdQueryValidator : AbstractValidator<GetBreedByIdQuery>
{
    public GetBreedByIdQueryValidator()
    {
        RuleFor(q => q.BreedId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsInvalid("BreedId"));
    }
}
