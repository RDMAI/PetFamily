using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Domain.Helpers;

namespace PetFamily.Application.SpeciesManagement.Queries.GetBreeds;

public class GetBreedsQueryValidator : AbstractValidator<GetBreedsQuery>
{
    public GetBreedsQueryValidator()
    {
        RuleFor(q => q.SpeciesId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsInvalid("SpeciesId"));

        RuleFor(q => q.Sort).MustBeValidSorting(typeof(BreedDTO));

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
