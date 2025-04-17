using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UpdatePet;

public record UpdatePetCommand(
    Guid VolunteerId,
    Guid PetId,
    UpdatePetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList) : ICommand;
