using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetById;

public class GetVolunteerByIdQueryValidator : AbstractValidator<GetVolunteerByIdQuery>
{
    public GetVolunteerByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsInvalid("Id"));
    }
}
