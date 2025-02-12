using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetColor
{
    public string Value { get; }

    public static Result<PetColor> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Result.Failure<PetColor>("Color cannot be empty.");

        return Result.Success(new PetColor(value));
    }

    private PetColor(string value)
    {
        Value = value;
    }
}
