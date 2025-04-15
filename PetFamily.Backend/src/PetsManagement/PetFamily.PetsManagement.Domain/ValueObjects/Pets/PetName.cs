using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Domain.ValueObjects.Pets;
public record PetName
{
    public string Value { get; }

    public static Result<PetName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Name");

        return new PetName(value);
    }

    private PetName(string value)
    {
        Value = value;
    }
}
