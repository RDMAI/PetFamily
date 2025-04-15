using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Domain.ValueObjects.Volunteers;

public record VolunteerFullName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string FatherName { get; }

    public override string ToString() => $"{LastName} {FirstName} {FatherName}";

    public static Result<VolunteerFullName, Error> Create(string firstName, string lastName, string fatherName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsNullOrEmpty("First name");
        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsNullOrEmpty("Last name");
        if (string.IsNullOrWhiteSpace(fatherName) || fatherName.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsNullOrEmpty("Father\'s name");

        return new VolunteerFullName(firstName, lastName, fatherName);
    }

    private VolunteerFullName(string firstName, string lastName, string fatherName)
    {
        FirstName = firstName;
        LastName = lastName;
        FatherName = fatherName;
    }
}
