using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

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
