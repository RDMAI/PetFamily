namespace PetFamily.Application.Shared.DTOs;

public record AddressDTO(
    string City,
    string Street,
    int HouseNumber,
    int? HouseSubNumber = null,
    int? AppartmentNumber = null);
