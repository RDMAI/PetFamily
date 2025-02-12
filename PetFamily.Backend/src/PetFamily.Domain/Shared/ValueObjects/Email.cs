using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared.ValueObjects;

public record Email
{
    public string Value { get; }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Result.Failure<Email>("Email cannot be empty.");

        return Result.Success(new Email(value));
    }

    private Email(string value)
    {
        Value = value;
    }
}
