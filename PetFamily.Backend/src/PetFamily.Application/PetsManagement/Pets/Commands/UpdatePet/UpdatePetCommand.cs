using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Pets.Commands.UpdatePet;

public record UpdatePetCommand(
    Guid VolunteerId,
    Guid PetId,
    UpdatePetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList) : ICommand;
