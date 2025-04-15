using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateMainInfo;
public class UpdateMainInfoCommandValidator : AbstractValidator<UpdateMainInfoCommand>
{
    public UpdateMainInfoCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => new
        {
            c.FirstName,
            c.LastName,
            c.FatherName
        }).MustBeValueObject(d => VolunteerFullName.Create(
            d.FirstName,
            d.LastName,
            d.FatherName));

        RuleFor(c => c.Email).MustBeValueObject(Email.Create);
        RuleFor(c => c.Description).MustBeValueObject(Description.Create);
        RuleFor(c => c.Phone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.ExperienceYears).MustBeValueObject(VolunteerExperienceYears.Create);
    }
}
