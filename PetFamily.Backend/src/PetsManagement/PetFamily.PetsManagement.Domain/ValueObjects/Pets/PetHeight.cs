using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Domain.ValueObjects.Pets;
public record PetHeight
{
    public float Value { get; }

    public static Result<PetHeight, Error> Create(float value)
    {
        if (value < 0)
            return ErrorHelper.General.ValueIsInvalid("Height");

        return new PetHeight(value);
    }

    private PetHeight(float value)
    {
        Value = value;
    }
}
