using Microsoft.AspNetCore.Http;

namespace PetFamily.PetsManagement.Contracts.Requests.Pets;

public record UploadPetPhotosRequest(IFormFileCollection Files);
