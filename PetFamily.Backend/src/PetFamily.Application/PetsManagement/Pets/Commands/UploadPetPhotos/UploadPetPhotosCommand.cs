using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Pets.Commands.UploadPetPhotos;

public record UploadPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<FileDTO> Photos) : ICommand;
