using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using System.IO;
namespace PetFamily.Domain.Shared.ValueObjects;

public record Phone
{
    public string Value { get; }

    public static Result<Phone, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorHelper.General.ValueIsNullOrEmpty("Phone");

        return new Phone(value);
    }

    private Phone(string value)
    {
        Value = value;
    }
}
