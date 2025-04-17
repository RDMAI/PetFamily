using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Domain.ValueObjects.Volunteers;
public record VolunteerExperienceYears
{
    public float Value { get; }

    public static Result<VolunteerExperienceYears, Error> Create(float value)
    {
        if (value < 0)
            return ErrorHelper.General.ValueIsInvalid("Experience in years");

        return new VolunteerExperienceYears(value);
    }

    private VolunteerExperienceYears(float value)
    {
        Value = value;
    }

    // EF Core
    private VolunteerExperienceYears() { }
}
