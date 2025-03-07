using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetsManagement.ValueObjects.Pets;

public record PetStatus
{
    public PetStatuses Value { get; }

    public static Result<PetStatus, Error> Create(PetStatuses value)
    {
        if (!Enum.IsDefined(typeof(PetStatuses), value))
            return ErrorHelper.General.ValueIsInvalid("Pet status");

        return new PetStatus(value);
    }

    private PetStatus(PetStatuses value)
    {
        Value = value;
    }
}

public enum PetStatuses
{
    NeedsHelp = 10,
    SeekingHome = 20,
    FoundHome = 30
}
