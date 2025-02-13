using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetsContext.ValueObjects.Volunteers;

public record VolunteerFullName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string FatherName { get; }

    public override string ToString() => $"{LastName} {FirstName} {FatherName}";

    public static Result<VolunteerFullName> Create(string firstName, string lastName, string fatherName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) return Result.Failure<VolunteerFullName>("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName)) return Result.Failure<VolunteerFullName>("Last name cannot be empty.");
        if (string.IsNullOrWhiteSpace(fatherName)) return Result.Failure<VolunteerFullName>("Father\'s name cannot be empty.");

        return Result.Success(new VolunteerFullName(firstName, lastName, fatherName));
    }

    private VolunteerFullName(string firstName, string lastName, string fatherName)
    {
        FirstName = firstName;
        LastName = lastName;
        FatherName = fatherName;
    }
}
