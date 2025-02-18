using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;

namespace PetFamily.Domain.Shared.ValueObjects;

public record Email
{
    public string Value { get; }

    public static Result<Email, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))  // add proper email validation
            return ErrorHelper.General.ValueIsNullOrEmpty("Email");

        return new Email(value);
    }

    private Email(string value)
    {
        Value = value;
    }
}
