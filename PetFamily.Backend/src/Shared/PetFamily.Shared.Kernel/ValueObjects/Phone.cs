using CSharpFunctionalExtensions;
namespace PetFamily.Shared.Kernel.ValueObjects;

public record Phone
{
    public const int MAX_LENGTH = 11;

    public string Value { get; }

    public static Result<Phone, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > MAX_LENGTH)  // add proper phone validation !!!!!
            return ErrorHelper.General.ValueIsInvalid("Phone");

        return new Phone(value);
    }

    private Phone(string value)
    {
        Value = value;
    }

    // EF Core
    private Phone() { }
}
