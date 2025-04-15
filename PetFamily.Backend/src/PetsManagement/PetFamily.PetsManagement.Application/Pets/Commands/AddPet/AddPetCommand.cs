using PetFamily.PetsManagement.Application.Pets.DTOs;

namespace PetFamily.PetsManagement.Application.Pets.Commands.AddPet;

public record AddPetCommand(
    Guid VolunteerId,
    AddPetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList) : ICommand;
