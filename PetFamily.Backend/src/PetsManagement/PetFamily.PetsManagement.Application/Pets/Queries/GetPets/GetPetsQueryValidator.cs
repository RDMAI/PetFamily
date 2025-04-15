using FluentValidation;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPets;

public class GetPetsQueryValidator : AbstractValidator<GetPetsQuery>
{
    public GetPetsQueryValidator()
    {
        RuleFor(q => q.Sort).MustBeValidSorting(typeof(PetDTO));

        RuleFor(q => q.CurrentPage)
            .GreaterThanOrEqualTo(1)
            .WithError(ErrorHelper.General.ValueIsInvalid("CurrentPage"));

        RuleFor(q => q.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithError(ErrorHelper.General.ValueIsInvalid("PageSize"));
        RuleFor(q => q.PageSize)
            .LessThanOrEqualTo(100)
            .WithError(ErrorHelper.General.ValueIsInvalid("PageSize"));
    }
}
