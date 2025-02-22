using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.SpeciesContext.ValueObjects;
public record SpeciesName
{
    public string Value { get; }

    public static Result<SpeciesName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Name");

        return new SpeciesName(value);
    }

    private SpeciesName(string value)
    {
        Value = value;
    }
}
