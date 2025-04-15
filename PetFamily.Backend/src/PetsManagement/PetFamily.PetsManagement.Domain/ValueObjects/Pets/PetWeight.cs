using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Domain.ValueObjects.Pets;
public record PetWeight
{
    public float Value { get; }

    public static Result<PetWeight, Error> Create(float value)
    {
        if (value < 0)
            return ErrorHelper.General.ValueIsInvalid("Weight");

        return new PetWeight(value);
    }

    private PetWeight(float value)
    {
        Value = value;
    }

    // EF Core
    private PetWeight() { }
}
