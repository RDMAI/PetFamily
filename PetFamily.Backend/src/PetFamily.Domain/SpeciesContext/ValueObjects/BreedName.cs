using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.SpeciesContext.ValueObjects;
public record BreedName
{
    public string Value { get; }

    public static Result<BreedName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Name");

        return new BreedName(value);
    }

    private BreedName(string value)
    {
        Value = value;
    }
}
