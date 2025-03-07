using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetsManagement.ValueObjects.Pets;

public record PetColor
{
    public string Value { get; }

    public static Result<PetColor, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Pet color");

        return new PetColor(value);
    }

    private PetColor(string value)
    {
        Value = value;
    }
}
