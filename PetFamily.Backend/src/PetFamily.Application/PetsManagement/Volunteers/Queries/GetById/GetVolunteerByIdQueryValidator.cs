using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;

public class GetVolunteerByIdQueryValidator : AbstractValidator<GetVolunteerByIdQuery>
{
    public GetVolunteerByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsInvalid("Id"));
    }
}
