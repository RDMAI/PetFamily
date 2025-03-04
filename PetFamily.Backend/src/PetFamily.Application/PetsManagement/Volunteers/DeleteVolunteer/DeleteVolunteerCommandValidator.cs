using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

namespace PetFamily.Application.PetsManagement.Volunteers.DeleteVolunteer;
public class DeleteVolunteerCommandValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));
    }
}
