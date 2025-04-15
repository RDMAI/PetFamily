using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Domain.ValueObjects.Pets;

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

    // EF Core
    private PetHealthInfo() {}
}
