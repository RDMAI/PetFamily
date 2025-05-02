using FluentValidation;
using PetFamily.PetsManagement.Domain.ValueObjects.Volunteers;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.CreateVolunteer;
public class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
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
