using CSharpFunctionalExtensions;

namespace PetFamily.Shared.Kernel.ValueObjects;
public record Description
{
    public string Value { get; }

    public static Result<Description, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_HIGH_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Description");

        return new Description(value);
    }

    private Description(string value)
    {
        Value = value;
    }

    // EF Core
    private Description() { }
}
