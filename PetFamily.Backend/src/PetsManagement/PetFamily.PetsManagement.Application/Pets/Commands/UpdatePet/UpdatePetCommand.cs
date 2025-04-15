using PetFamily.Application.Shared.DTOs;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UpdatePet;

public record UpdatePetCommand(
    Guid VolunteerId,
    Guid PetId,
    UpdatePetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList) : ICommand;
