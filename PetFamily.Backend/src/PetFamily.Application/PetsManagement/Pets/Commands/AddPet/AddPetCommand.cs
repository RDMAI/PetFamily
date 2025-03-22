using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Pets.Commands.AddPet;

public record AddPetCommand(
    Guid VolunteerId,
    AddPetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList) : ICommand;
