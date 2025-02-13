using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetHealthInfo
{
    public string Value { get; }

    public static Result<PetHealthInfo> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Result.Failure<PetHealthInfo>("Health info cannot be empty.");

        return Result.Success(new PetHealthInfo(value));
    }

    private PetHealthInfo(string value)
    {
        Value = value;
    }
}
