using CSharpFunctionalExtensions;

namespace PetFamily.Shared.Kernel.ValueObjects;

public record Address
{
    public string City { get; }
    public string Street { get; }
    public int HouseNumber { get; }
    public int? HouseSubNumber { get; }
    public int? AppartmentNumber { get; }

    //public string AddressAsString => $"{City}, {Street} {HouseNumber}/{HouseSubNumber}, {AppartmentNumber}";

    public static Result<Address, Error> Create(string city, string street, int houseNumber, int? houseSubNumber = null, int? apartmentNumber = null)
    {
        if (string.IsNullOrWhiteSpace(city) || city.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("City");
        if (string.IsNullOrWhiteSpace(street) || street.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Street");
        if (houseNumber <= 0)
            return ErrorHelper.General.ValueIsInvalid("House number");
        if (houseSubNumber != null && houseSubNumber <= 0)
            return ErrorHelper.General.ValueIsInvalid("House subnumber");
        if (apartmentNumber != null && apartmentNumber <= 0)
            return ErrorHelper.General.ValueIsInvalid("Apartment number");

        return new Address(city, street, houseNumber, houseSubNumber, apartmentNumber);
    }

    private Address(string city, string street, int houseNumber, int? houseSubNumber, int? appartmentNumber)
    {
        City = city;
        Street = street;
        HouseNumber = houseNumber;
        HouseSubNumber = houseSubNumber;
        AppartmentNumber = appartmentNumber;
    }

    // EF Core
    private Address() { }
}
