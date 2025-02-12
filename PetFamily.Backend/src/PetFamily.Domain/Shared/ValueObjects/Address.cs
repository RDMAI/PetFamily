using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared.ValueObjects;

public record Address
{
    public string City { get; }
    public string Street { get; }
    public int HouseNumber { get; }
    public int? HouseSubNumber { get; }
    public int? AppartmentNumber { get; }

    //public string AddressAsString => $"{City}, {Street} {HouseNumber}/{HouseSubNumber}, {AppartmentNumber}";

    public static Result<Address> Create(string city, string street, int houseNumber, int? houseSubNumber = null, int? appartmentNumber = null)
    {
        if (string.IsNullOrWhiteSpace(city)) return Result.Failure<Address>("City name cannot be empty.");
        if (string.IsNullOrWhiteSpace(street)) return Result.Failure<Address>("Street name cannot be empty.");
        if (houseNumber <= 0) return Result.Failure<Address>("House number should be more than zero.");
        if (houseSubNumber != null && houseSubNumber <= 0) return Result.Failure<Address>("House sub number should be more than zero.");
        if (appartmentNumber != null && appartmentNumber <= 0) return Result.Failure<Address>("Appartment number should be more than zero.");

        return Result.Success(new Address(city, street, houseNumber, houseSubNumber, appartmentNumber));
    }

    private Address(string city, string street, int houseNumber, int? houseSubNumber, int? appartmentNumber)
    {
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        HouseSubNumber = houseSubNumber;
        AppartmentNumber = appartmentNumber;
    }
}
