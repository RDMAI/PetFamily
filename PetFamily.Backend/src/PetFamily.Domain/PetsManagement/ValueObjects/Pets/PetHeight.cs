using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;

namespace PetFamily.Domain.Shared.ValueObjects;
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
