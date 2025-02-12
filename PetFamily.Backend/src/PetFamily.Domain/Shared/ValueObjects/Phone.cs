using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.ValueObjects;

public record Phone
{
    public string Value { get; }

    public static Result<Phone> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Result.Failure<Phone>("Phone cannot be empty.");

        return Result.Success(new Phone(value));
    }

    private Phone(string value)
    {
        Value = value;
    }
}
