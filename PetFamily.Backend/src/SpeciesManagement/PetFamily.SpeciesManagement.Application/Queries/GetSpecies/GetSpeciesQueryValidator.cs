﻿using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;
using PetFamily.SpeciesManagement.Application.DTOs;

namespace PetFamily.SpeciesManagement.Application.Queries.GetSpecies;

public class GetSpeciesQueryValidator : AbstractValidator<GetSpeciesQuery>
{
    public GetSpeciesQueryValidator()
    {
        RuleFor(q => q.Sort).MustBeValidSorting(typeof(SpeciesDTO));

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
