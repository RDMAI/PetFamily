using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.PetsManagement.Domain.ValueObjects.Pets;
public record PetSerialNumber
{
    public int Value { get; }

    public static Result<PetSerialNumber, Error> Create(int value)
    {
        if (value <= 0)
            return ErrorHelper.General.ValueIsInvalid("SerialNumber");

        return new PetSerialNumber(value);
    }

    private PetSerialNumber(int value)
    {
        Value = value;
    }

    public static bool operator >(PetSerialNumber a, PetSerialNumber b)
        => a.Value > b.Value;
    public static bool operator <(PetSerialNumber a, PetSerialNumber b)
        => a.Value < b.Value;
    public static bool operator >=(PetSerialNumber a, PetSerialNumber b)
        => a.Value >= b.Value;
    public static bool operator <=(PetSerialNumber a, PetSerialNumber b)
        => a.Value <= b.Value;

    public static bool operator >(PetSerialNumber a, int b)
        => a.Value > b;
    public static bool operator <(PetSerialNumber a, int b)
        => a.Value < b;
    public static bool operator >=(PetSerialNumber a, int b)
        => a.Value >= b;
    public static bool operator <=(PetSerialNumber a, int b)
        => a.Value <= b;
}
