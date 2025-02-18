using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetStatus
{
    public Statuses Value { get; }

    public static Result<PetStatus, Error> Create(Statuses value)
        => new PetStatus(value);

    private PetStatus(Statuses value)
    {
        Value = value;
    }

    public enum Statuses
    {
        NeedsHelp = 10,
        SeekingHome = 20,
        FoundHome = 30
    }
}
