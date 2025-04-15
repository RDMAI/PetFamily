using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.SpeciesManagement.Domain.ValueObjects;
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
