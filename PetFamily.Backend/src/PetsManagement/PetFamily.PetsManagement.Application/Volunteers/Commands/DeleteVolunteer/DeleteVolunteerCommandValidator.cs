using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.DeleteVolunteer;
public class DeleteVolunteerCommandValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));
    }
}
