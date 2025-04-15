using PetFamily.Application.Shared.DTOs;
using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UploadPetPhotos;

public record UploadPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<FileDTO> Photos) : ICommand;
