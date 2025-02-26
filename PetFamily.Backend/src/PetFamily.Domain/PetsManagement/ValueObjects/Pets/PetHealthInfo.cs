using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetHealthInfo
{
    public string Value { get; }

    public static Result<PetHealthInfo, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Health information");

        return new PetHealthInfo(value);
    }

    private PetHealthInfo(string value)
    {
        Value = value;
    }
}
