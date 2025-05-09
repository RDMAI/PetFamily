﻿using FluentValidation;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetVolunteers;

public class GetVolunteersQueryValidator : AbstractValidator<GetVolunteersQuery>
{
    public GetVolunteersQueryValidator()
    {
        RuleFor(q => q.Sort).MustBeValidSorting(typeof(VolunteerDTO));

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
